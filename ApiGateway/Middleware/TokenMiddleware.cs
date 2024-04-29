using Core.Utilities;
using Newtonsoft.Json;

namespace ApiGateway.Middleware
{
    public class TokenMiddleware : IMiddleware
    {
        private readonly JwtService _jwtService;

        public TokenMiddleware(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (ShouldProcessRequest(context)){
                var originalBody = context.Response.Body;

                try
                {
                    using var responseBody = new MemoryStream();
                    // Replace the response body with our MemoryStream
                    context.Response.Body = responseBody;

                    // Call the next middleware in the pipeline
                    await next(context);

                    // Rewind the MemoryStream and read its content
                    responseBody.Seek(0, SeekOrigin.Begin);
                    var responseBodyString = await new StreamReader(responseBody).ReadToEndAsync();

                    // Deserialize the response body to JSON
                    var responseBodyJson = JsonConvert.DeserializeObject<UserResponse>(responseBodyString);

                    // Generate the token using the response data
                    var token = _jwtService.GenerateAccessToken(responseBodyJson.Id, responseBodyJson.FirstName, responseBodyJson.FamilyName);
                    var refresh = _jwtService.GenerateRefreshToken(responseBodyJson.Id, responseBodyJson.FirstName, responseBodyJson.FamilyName);
                    var tokenResponse = JsonConvert.SerializeObject(new { accessToken = token, refreshToken = refresh });

                    // Write the token response to the original response body
                    using var newStream = new MemoryStream();
                    var sw = new StreamWriter(newStream);
                    sw.Write(tokenResponse);
                    sw.Flush();

                    newStream.Seek(0, SeekOrigin.Begin);

                    await newStream.CopyToAsync(originalBody);
                }
                catch
                {
                    // Restore the original response body
                    context.Response.Body = originalBody;
                }
            }
            else
            {
                await next(context);
            }
        }

        private bool ShouldProcessRequest(HttpContext context)
        {
            if (context.Request.Path.Equals("/v1/auth"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private class UserResponse
        {
            public string Id { get; set; }
            public string FirstName { get; set; }
            public string FamilyName { get; set; }
        }
    }
}