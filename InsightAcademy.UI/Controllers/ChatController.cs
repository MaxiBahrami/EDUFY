using InsightAcademy.ApplicationLayer.Services;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Enums;
using InsightAcademy.InfrastructureLayer.Data.Seed;
using InsightAcademy.UI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;

namespace InsightAcademy.UI.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService _chat;
        private readonly IUserService _userService;

        public ChatController(IChatService chat, IUserService userService)
        {
            _chat = chat;
            _userService = userService;
        }

        public async Task<IActionResult> Inbox()
        {
            var recepients = await _chat.GetRecepients(User.GetUserId());
            var currentUser = await _userService.GetUserDetailById(User.GetUserId());
            ViewBag.Recepients = recepients;
            ViewBag.CurrentUserName = currentUser.FullName;
            ViewBag.CurrentUserPicture = currentUser.ProfileImageUrl;
            return View(recepients);
        }

        public async Task<IActionResult> SendMessage(Chat chat)
        {
            chat.SenderId = User.GetUserId();

            var recipient = await _userService.GetUserDetailById(chat.RecipientId);
            var sender = await _userService.GetUserDetailById(chat.SenderId);

            chat.RecipientId = recipient.Id;
            chat.SenderId = sender.Id;
            chat.SenderUserProfileUrl = sender.ProfileImageUrl;
            chat.RecipientUserProfileUrl = recipient.ProfileImageUrl;
            chat.RecipientUserName = recipient.FullName;
            chat.SenderUserName = sender.FullName;
            chat.CreatedDateTime = DateTime.Now;
            chat.Type = MessageTypeEnum.Text;
            var chatObj = await _chat.AddMessage(chat);
            if (chatObj != null)
            {
                return Json(new { status = true, message = "" });
            }
            else
            {
                return Json(new { status = false, message = "Server Error!" });
            }
        }

        public async Task<IActionResult> GetMediaFile(string groupName)
        {
            var files = await _chat.GetMediaFile(groupName);
            return new JsonResult(files);
        }

        public async Task<IActionResult> GetMediaImages(string groupName)
        {
            var files = await _chat.GetMediaImages(groupName);
            return new JsonResult(files);
        }

        public async Task<IActionResult> GetMediaVideos(string groupName)
        {
            var videos = await _chat.GetMediaVideos(groupName);
            return new JsonResult(videos);
        }

        public async Task<IActionResult> GetMediaLinks(string groupName)
        {
            var videos = await _chat.GetMediaLinks(groupName);
            return new JsonResult(videos);
        }
    }
}