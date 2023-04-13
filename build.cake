#addin nuget:?package=Cake.Unity&version=0.9.0
#addin nuget:?package=WTelegramClient&version=3.3.3
#addin nuget:?package=FluentFTP&version=46.0.2
#addin nuget:?package=Cake.Git&version=3.0.0

using System.ComponentModel.Design.Serialization;
using TL;
using FluentFTP;
using System.IO;
using System.Text.RegularExpressions;

const string ProjectName = "RaftWars";
const string ArtifactsFolderPath = "./artifacts";
const string ProjectFolderPath = $"./src/{ProjectName}";
const string UnityBuildMethod = "Editor.Builder.BuildWebGl";
const string SearchingBuildFolderPattern = @"^.{0,}_" + ProjectName + "_.{0,}$";
const string UnityPath = @"D:\Soft\Unity\2021.3.15f1\Editor\Unity.exe";

var target = Argument("target", "Send-Build-Notification");

string TelegramSessionPath => Context.Configuration.GetValue("Telegram_SessionPath");

Task("Clean-Artifacts")
    .Does(() => 
{
    CleanDirectory(ArtifactsFolderPath);
});

Task("Build-WebGL")
    .IsDependentOn("Clean-Artifacts")
    .Does(() => 
{
    UnityEditor(UnityPath,
        CreateUnityEditorArguments(),
        new UnityEditorSettings 
        {
            RealTimeLog = true
        });
});

UnityEditorArguments CreateUnityEditorArguments()
{
    var arguments = new UnityEditorArguments
    {
        LogFile = ArtifactsFolderPath + "/unity.log",
        ExecuteMethod = UnityBuildMethod,
        BuildTarget = BuildTarget.WebGL,
        ProjectPath = ProjectFolderPath
    };
    arguments.Custom.BuildFolder = CreateBuildFolderName();
    return arguments;
}

Task("Send-Build-Notification")
    .IsDependentOn("UploadFile")
    .Does(async () => 
{
    Int64 chatId = Int64.Parse(Context.Configuration.GetValue("Telegram_ChatId"));
    using var tgClient = new WTelegram.Client(TelegramConfig, System.IO.File.Open(TelegramSessionPath, FileMode.OpenOrCreate));
    var account = await tgClient.LoginUserIfNeeded();
    var dialogs = await tgClient.Messages_GetAllChats();
    var dialog = dialogs.chats[chatId];
    await tgClient.SendMessageAsync(dialog, CreateTelegramMessage());
});

string CreateTelegramMessage()
{
    var lastCommit = GitLog(new DirectoryPath("."), 1).First();
    var message = $"Новый билд по проекту {ProjectName}! " + 
        $"Произошедшие изменения: {lastCommit.Message}\n" +
        $"Демонстрационная ссылка: https://immgames.ru/Games/Wolf/{ProjectName}. "+
        $"Внимание! После следующего обновления эта версия игры \"сгорит\" из ссылки";
    return message;
}

Task("UploadFile")
    .IsDependentOn("Build-WebGL")
    .Does(() => 
{
    var client = new FtpClient(
        FtpConfig("Ftp_Host"), 
        FtpConfig("Ftp_Login"), 
        FtpConfig("Ftp_Password"));
    client.Connect();
    var targetDirectory = GetLastArtifactDirectory();        
    client.DeleteDirectory($"/{ProjectName}");
    client.CreateDirectory($"/{ProjectName}");
    client.UploadDirectory(targetDirectory, $"/{ProjectName}");
});

RunTarget(target);

string GetLastArtifactDirectory() => 
    System.IO.Directory.EnumerateDirectories(".\\artifacts")
        .First(directory => Regex.IsMatch(directory, SearchingBuildFolderPattern));

string FtpConfig(string key) => key switch
{
    "Ftp_Host" => Context.Configuration.GetValue("Ftp_Host"),
    "Ftp_Login" => Context.Configuration.GetValue("Ftp_Login"),
    "Ftp_Password" => Context.Configuration.GetValue("Ftp_Password")
};

string CreateBuildFolderName()
{
    return $"{DateTime.Today:d}_{ProjectName}_{DateTime.Now.Hour}_{DateTime.Now.Minute}";
}

string TelegramConfig(string what)
{
    switch (what)
    {
        case "api_id": return Context.Configuration.GetValue("Telegram_ApiId");
        case "api_hash": return Context.Configuration.GetValue("Telegram_ApiHash");
        case "phone_number": return Context.Configuration.GetValue("Telegram_PhoneNumber");
        case "verification_code": Console.Write("Code: "); return Console.ReadLine();
        // Есть включена двойная аутентификация на аккаунте это может понадобиться:
        case "first_name": return "John";
        case "last_name": return "Doe";
        case "password": return "secret!";
        default: return null;                  
    }
}