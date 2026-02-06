using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieStoreMvc6.Models.Domain;
using System.Security.Claims;

namespace MovieStoreMvc6
{
    public class Helper : Controller
    {
      
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContext;
        protected readonly string _userid;
        protected readonly string _role;
        public Helper(IHttpContextAccessor _accessor,IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            this._environment = environment;
            _httpContext = _accessor;
            var httpcontext_access = _accessor.HttpContext;
            _userid = httpcontext_access.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _role = httpcontext_access.User.FindFirst(ClaimTypes.Role).Value;


        }

       
     
        [NonAction]
        public Tuple<int, string> SaveImage(IFormFile imageFile)
        {
            try
            {
                string _moviFolder = _configuration.GetSection("CustomPathSection")["MoviesFolder"];
                var wwwPath = this._environment.WebRootPath;
                var path = Path.Combine(wwwPath, _moviFolder);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Check the allowed extenstions
                var ext = Path.GetExtension(imageFile.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                if (!allowedExtensions.Contains(ext))
                {
                    string msg = string.Format("Only {0} extensions are allowed", string.Join(",", allowedExtensions));
                    return new Tuple<int, string>(0, msg);
                }
                string uniqueString = Guid.NewGuid().ToString();
                // we are trying to create a unique filename here
                var newFileName = uniqueString + ext;
                var fileWithPath = Path.Combine(path, newFileName);
                var stream = new FileStream(fileWithPath, FileMode.Create);
                imageFile.CopyTo(stream);
                stream.Close();
                return new Tuple<int, string>(1, newFileName);
            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(0, "Error has occured");
            }
        }
        public bool DeleteImage(string imageFileName)
        {
            try
            {
                string _moviFolder = _configuration.GetSection("CustomPathSection")["MoviesFolder"];
                var wwwPath = this._environment.WebRootPath;
                var path = Path.Combine(wwwPath, _moviFolder+"\\", imageFileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
