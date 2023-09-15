using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Http; // Thêm namespace này
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Facebook;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authentication.Twitter;
using System.Web;
using LinqToTwitter.OAuth;
using LinqToTwitter;

namespace BackENd.Controllers
{
    [Route("api/auth")]
    [ApiController]

    public class FacebookTokenResponse
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
    }

    public class FacebookUserData
    {
        public string Id { get; set; }
        public string Email { get; set; }
        // Các thuộc tính khác từ dữ liệu trả về từ Facebook
    }

    public class AuthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IHttpClientFactory httpClientFactory, ILogger<AuthController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;

        }

        public class UserInfoDto
        {
            public string Id { get; set; }
            public string Email { get; set; }
            // Thêm các thuộc tính khác của người dùng cần trả về
        }



        [HttpGet("facebook-login")]
        public IActionResult FacebookLogin()
        {

            var redirectUrl = "https://www.facebook.com/v12.0/dialog/oauth" +
                $"?client_id=689038589935930" +
                "&redirect_uri=https://localhost:7153/facebook-callback" +
                "&response_type=code" +
                "&scope=email";
            _logger.LogInformation("Đã xử lý login thành công");
            return Ok(new { RedirectUrl = redirectUrl });
        }

        [HttpGet("facebook-callback")]
        public async Task<IActionResult> FacebookCallback([FromQuery] string code)
        {
            try
            {
               

                // Chuyển hướng người dùng đến trang React sau khi đăng nhập
                return Redirect("http://localhost:3000");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Xảy ra lỗi khi xử lý callback từ Facebook");
                // Xử lý lỗi và chuyển hướng hoặc trả về trang lỗi
                return Redirect("http://localhost:3000/error");
            }
        }

        [HttpGet("get-user-info")]
        public IActionResult GetUserInfo()
        {
            _logger.LogInformation("Đã xử lý get-user-info thành công");
            var userInfoJson = HttpContext.Session.GetString("UserInfo");

            if (!string.IsNullOrEmpty(userInfoJson))
            {
                // Chuyển đổi JSON thành đối tượng UserInfoDto (hoặc đối tượng tương tự)
                var userInfo = JsonConvert.DeserializeObject<UserInfoDto>(userInfoJson);

                // Trả về thông tin người dùng
                return Ok(userInfo);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = "https://accounts.google.com/o/oauth2/v2/auth" +
                $"?client_id=336646770475-6fc0bm1uuq2fldtcrm4eo2ge4s55o05e.apps.googleusercontent.com" + // Thay YOUR_GOOGLE_CLIENT_ID bằng Client ID của bạn
                "&redirect_uri=https://localhost:7153/google-callback" + // Thay đổi URI theo cấu hình của bạn
                "&response_type=code" +
                "&scope=email";

            _logger.LogInformation("Đã xử lý login google thành công");
            return Ok(new { RedirectUrl = redirectUrl });
        }


        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback([FromQuery] string code)
        {
            try
            {
               

                return Redirect("http://localhost:3000");

            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return BadRequest("Failed to retrieve user information");
            }
        }

        [HttpGet("get-user-google")]
        public async Task<IActionResult> GetUserGoogle()
        {
            try
            {
                // Đọc dữ liệu từ phần tử JSON trả về
                var userData = HttpContext.Session.GetString("UserData");
                _logger.LogInformation("Get User: ", userData);
                if (!string.IsNullOrEmpty(userData))
                {
                    return Ok(userData);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return BadRequest("Failed to retrieve user information");
            }
        }

        [HttpGet("twitter-login")]

        public IActionResult TwitterLogin()
        {
           

            var apiKey = "L3DHX8ePw3KmmFgJDVHC6SvOi";
            var apiSecret = "hYamPM564IxdgZbNfSswmtHFSBdqarzgkfffsLlqxDtm5hmB5Z";
            var accessToken = "1554454328358088707-2dHsmK0x1cs1mqkYYAbPMIaXSPXByS";
            var accessTokenSecret = "V5A94pLZ51EmcBgb6ZfKz2y3HLek4cnhTM9YczGgIzXoC";

            // Khởi tạo một phiên làm việc Twitter
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = apiKey,
                    ConsumerSecret = apiSecret,
                     OAuthToken = accessToken,
                    OAuthTokenSecret = accessTokenSecret
                }
            };
           
            // Chứng thực bằng OAuth
            auth.AuthorizeAsync().Wait();

            // Tạo URL xác thực Twitter
            var twitterUrl = "https://twitter.com/oauth/authenticate?oauth_token=" + auth.CredentialStore.OAuthToken;

            // Chuyển hướng người dùng đến trang xác thực Twitter
            return Redirect(twitterUrl);
        }

        // Bước 3: Xử lý callback từ Twitter
        [HttpGet("twitter-callback")]
        public async Task<IActionResult> TwitterCallback([FromQuery] string oauth_token, [FromQuery] string oauth_verifier)
        {
           

            // Bước 5: Bây giờ bạn có thể sử dụng accessToken và accessTokenSecret để truy cập tài khoản Twitter của người dùng

            return Redirect("http://localhost:3000");
        }
    }


}
