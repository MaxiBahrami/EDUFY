using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.ApplicationLayer.Services;
using InsightAcademy.DomainLayer.Entities.Identity;
using InsightAcademy.UI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace InsightAcademy.UI.Hubs
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;
        private readonly UserManager<ApplicationUser> _userManager;

        public PresenceHub(PresenceTracker tracker, UserManager<ApplicationUser> userManager)
        {
            _tracker = tracker;
            _userManager = userManager;
        }

        public override async Task OnConnectedAsync()
        {
            var isOnline = await _tracker.UserConnected(Context.User.GetUserId(), Context.ConnectionId);

            if (isOnline)
            {
                await Clients.Others.SendAsync("UserIsOnline", "");
            }

            //var currentUsers = await _tracker.GetOnlineUsers();
            //await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var isOffline = await _tracker.UserDisConnected(Context.User.GetUserId(), Context.ConnectionId);
            if (isOffline)
            {
                await Clients.Others.SendAsync("UserIsOffline", "");
            }

            //var currentUsers = await _tracker.GetOnlineUsers();
            //await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
            var user = await _userManager.FindByEmailAsync(Context.User.Identity.Name);
            if (user != null)
            {
                user.LastOnline = DateTime.Now;
                await _userManager.UpdateAsync(user);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}