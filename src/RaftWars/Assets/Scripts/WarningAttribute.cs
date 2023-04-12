using System;

namespace Common
{
    [AttributeUsage(validOn: AttributeTargets.All)]
    public class WarningAttribute : Attribute
    {
        public WarningAttribute(string warning)
        {
            
        }
    }
}