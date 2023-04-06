using RaftWars.Infrastructure;
using Skins;
using UnityEngine;

namespace Infrastructure.States
{
    public class RepaintPlayerState : IPayloadedState<PlayerColors>
    {
        public void Enter(PlayerColors material)
        {
            Game.PlayerService.RepaintWith(material);
        }

        public void Exit()
        {
            
        }
    }
}