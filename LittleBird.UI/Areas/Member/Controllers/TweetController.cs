﻿using LittleBird.Model.Option;
using LittleBird.Service.Option;
using LittleBird.UI.Areas.Member.Models.DTO;
using LittleBird.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LittleBird.UI.Areas.Member.Controllers
{
    public class TweetController : Controller
    {
        AppUserService _appUserService;
        LikeService _likeService;
        TweetService _tweetService;
        CommentService _commentService;
        public TweetController()
        {
            _appUserService = new AppUserService();
            _likeService = new LikeService();
            _tweetService = new TweetService();
            _commentService = new CommentService();
        }
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Add(Tweet data, HttpPostedFileBase Image)
        {
            List<string> UploadedImagePaths = new List<string>();

            UploadedImagePaths = ImageUploader.UploadSingleImage(ImageUploader.OriginalProfileImagePath, Image, 1);

            data.ImagePath = UploadedImagePaths[0];

            if (data.ImagePath == "0" || data.ImagePath == "1" || data.ImagePath == "2")
            {
                data.ImagePath = ImageUploader.DefaultProfileImagePath;
                data.ImagePath = ImageUploader.DefaultXSmallProfileImage;
                data.ImagePath = ImageUploader.DefaulCruptedProfileImage;
            }
            else
            {
                data.ImagePath = UploadedImagePaths[1];
                data.ImagePath = UploadedImagePaths[2];
            }

            AppUser user = _appUserService.GetByDefault(x => x.UserName == User.Identity.Name);
            data.AppUserID = user.ID;
            data.PublishDate = DateTime.Now;

            _tweetService.Add(data);
            return Redirect("/Member/Tweet/List");


        }
        public ActionResult List()
        {
            Guid userID = _appUserService.FindByUserName(User.Identity.Name).ID;
            List<Tweet> model = _tweetService.GetDefault(x => x.AppUserID == userID && (x.Status == Core.Enum.Status.Active || x.Status == Core.Enum.Status.Updated));
            return View(model);
        }
        public ActionResult Update(Guid id)
        {
            AddTweetDTO model = new AddTweetDTO();
            Tweet tweet = _tweetService.GetByID(id);
           
            model.id= tweet.ID;
        
            model.TweetContent = tweet.TweetContent;
            model.PublishDate = DateTime.Now;
            model.ImagePath= tweet.ImagePath;
          

            return View(model);
        }
        [HttpPost]
        public ActionResult Update(AddTweetDTO data,HttpPostedFileBase Image)
        {

            List<string> UploadedImagePaths = new List<string>();

            UploadedImagePaths = ImageUploader.UploadSingleImage(ImageUploader.OriginalProfileImagePath, Image, 1);

            data.ImagePath = UploadedImagePaths[0];

            Tweet update = _tweetService.GetByID(data.id);

            if (data.ImagePath == "0" || data.ImagePath == "1" || data.ImagePath == "2")
            {

                if (update.ImagePath == null || update.ImagePath == ImageUploader.DefaultProfileImagePath)
                {
                    update.ImagePath = ImageUploader.DefaultProfileImagePath;
                    update.ImagePath = ImageUploader.DefaultXSmallProfileImage;
                    update.ImagePath = ImageUploader.DefaulCruptedProfileImage;
                }
                else
                {
                    update.ImagePath = update.ImagePath;
                }

            }
            else
            {
                update.ImagePath = UploadedImagePaths[0];
                update.ImagePath = UploadedImagePaths[1];
                update.ImagePath = UploadedImagePaths[2];
            }

            update.TweetContent = data.TweetContent;
            update.PublishDate = data.PublishDate;
            update.Status = Core.Enum.Status.Updated;
            _tweetService.Add(update);
            return Redirect("/Member/Tweet/List");
        }
        public ActionResult Delete(Guid id)
        {
            _tweetService.Remove(id);
            return Redirect("/Author/Article/List");
        }
    }

        
}