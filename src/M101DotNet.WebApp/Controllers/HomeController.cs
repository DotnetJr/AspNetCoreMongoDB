using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using M101DotNet.WebApp.Data;
using M101DotNet.WebApp.Models;
using M101DotNet.WebApp.Models.HomeViewModels;

namespace M101DotNet.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private MongoDbSettings _dbSettings { get; set; }
        private readonly ILogger _logger;

        public HomeController(UserManager<ApplicationUser> userManager, 
                              IOptions<MongoDbSettings> settings, 
                              ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _dbSettings = settings.Value;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // MongoDB Context
            var blogContext = new MongoBlogContext(_dbSettings);
            
            // XXX WORK HERE
            // find the most recent 10 posts and order them
            // from newest to oldest
            var col = blogContext.Posts;
            var filter = new BsonDocument();
            var recentPosts = await col.Find(filter)
                                       .Sort(Builders<Post>.Sort.Descending(x => x.CreatedAtUtc))
                                       .Limit(10)
                                       .ToListAsync();

            var model = new IndexModel
            {
                RecentPosts = recentPosts
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult NewPost()
        {
            return View(new NewPostModel());
        }

        [HttpPost]
        public async Task<IActionResult> NewPost(NewPostModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get Current User
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // MongoDB Context
            var blogContext = new MongoBlogContext(_dbSettings);

            // XXX WORK HERE
            // Insert the post into the posts collection
            var col = blogContext.Posts;
            var post = new Post
            {
                Author = user.UserName,
                Title = model.Title,
                Content = model.Content,
                CreatedAtUtc = DateTime.UtcNow,
                Comments = new List<Comment>()
            };

            string[] tags = model.Tags.Split(';');
            post.Tags = new List<string>();
            foreach (string tag in tags)
            {
                post.Tags.Add(tag);  
            }

            await col.InsertOneAsync(post);

            return RedirectToAction("Post", new { id = post.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Post(string id)
        {
            // MongoDB Context
            var blogContext = new MongoBlogContext(_dbSettings);

            // XXX WORK HERE
            // Find the post with the given identifier
            var col = blogContext.Posts;
            var filter = Builders<Post>.Filter.Eq(x => x.Id, ObjectId.Parse(id));
            var post = await col.Find(filter).FirstOrDefaultAsync();

            if (post == null)
            {
                return RedirectToAction("Index");
            }

            var model = new PostModel
            {
                Post = post
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Posts(string tag = null)
        {
            // MongoDB Context
            var blogContext = new MongoBlogContext(_dbSettings);

            // XXX WORK HERE
            // Find all the posts with the given tag if it exists.
            // Otherwise, return all the posts.
            // Each of these results should be in descending order.
            var col = blogContext.Posts;

            BsonDocument filter = new BsonDocument();
            if (tag != null)
            {
                filter.Add("Tags", tag);
                ViewData["Tags"] = tag;
            }
            
            var posts = await col.Find(filter)
                                 .Sort(Builders<Post>.Sort.Descending(x => x.CreatedAtUtc))
                                 .Limit(20)
                                 .ToListAsync();

            return View(posts);
        }

        [HttpPost]
        public async Task<IActionResult> NewComment(NewCommentModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Post", new { id = model.PostId });
            }

            // Get Current User
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // MongoDB Context
            var blogContext = new MongoBlogContext(_dbSettings);

            // XXX WORK HERE
            // add a comment to the post identified by model.PostId.
            // you can get the author from "this.User.Identity.Name"
            var col = blogContext.Posts;
            var comment = new Comment
            {
                Author = user.UserName,
                Content = model.Content,
                CreatedAtUtc = DateTime.UtcNow
            };

            var filter = Builders<Post>.Filter.Eq(x => x.Id, ObjectId.Parse(model.PostId));

            // Push = Add Items in the Json
            // Pull = Remove Items from the Json 
            var update = Builders<Post>.Update.Push(x => x.Comments, comment);

            await col.UpdateOneAsync(filter, update);

            return RedirectToAction("Post", new { id = model.PostId });
        }

        [HttpPost]
        public async Task<IActionResult> CommentLike(CommentLikeModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Post", new { id = model.PostId });
            }
            
            // MongoDB Context
            var blogContext = new MongoBlogContext(_dbSettings);

            // XXX WORK HERE
            // Increment the Likes field for the comment at {model.Index}
            // inside the post {model.PostId}.
            //
            // NOTE: The 2.0.0 driver has a bug in the expression parser and 
            // might throw an exception depending on how you solve this problem. 
            // This is documented here along with a workaround:
            // https://jira.mongodb.org/browse/CSHARP-1246
            var fieldName = string.Format("Comments.{0}.Likes", model.Index);

            await blogContext.Posts.UpdateOneAsync(x => x.Id == ObjectId.Parse(model.PostId),
                                                   Builders<Post>.Update.Inc(fieldName, 1));


            return RedirectToAction("Post", new { id = model.PostId });
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
