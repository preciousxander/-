using XanderFront;
using ClassDataBaseLibrary;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Components.Forms;
//using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

builder.Services.AddScoped(
    hc => new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7227/")
    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddCookie(config =>
    {
        config.Cookie.Name = "jwt_token";
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true
        };

        options.Events = new JwtBearerEvents()
        {
            //get cookie value
            OnMessageReceived = context =>
            {
                var token = "";
                context.Request.Cookies.TryGetValue("jwt_token", out token);
                context.Token = token;
                return Task.CompletedTask;
            }
        };
    });


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSession();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

//Add JWToken to all incoming HTTP Request Header
app.Use(async (context, next) =>
{
    var JWToken = context.Session.GetString("tokenKey");
    if (!string.IsNullOrEmpty(JWToken))
    {
        context.Request.Headers.Add("Authorization", "Bearer " + JWToken);
    }
    await next();
});


app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

string jsonMessage(object msg)
{
    return Newtonsoft.Json.JsonConvert.SerializeObject(msg);
}

app.MapPost("Register", async (User user, HttpContext context, HttpClient http) =>
{
    var newUser = new StringContent(
        JsonSerializer.Serialize(user),
        Encoding.UTF8,
        "application/json");

    using var httpResponseMessage = await http.PostAsync("/auth/Register", newUser);

    if (httpResponseMessage.IsSuccessStatusCode)
    {
        //context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(jsonMessage(new { message = "Ok" }));
        return;
    }

    await context.Response.WriteAsync(jsonMessage($"Failed to register new user - {httpResponseMessage.StatusCode}"));
});


app.MapPost("Login", async (User user, HttpContext context, HttpClient http) =>
{
    var newUser = new StringContent(
        JsonSerializer.Serialize(user),
        Encoding.UTF8,
        "application/json");

    using var httpResponseMessage = await http.PostAsync("/auth/Login", newUser);

    if (httpResponseMessage.IsSuccessStatusCode)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Login) };
        // создаем JWT-токен
        var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(666)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

        var jwtKey = new JwtSecurityTokenHandler().WriteToken(jwt);

        context.Session.SetString("tokenKey", jwtKey);
        await context.Response.WriteAsync(jsonMessage(new { message = "Ok", token = jwtKey }));
        return;
    }

    await context.Response.WriteAsync(jsonMessage($"Failed to login - {httpResponseMessage.StatusCode}"));
});


app.MapPost("CardStudent", async (HttpContext context) =>
{
    var restClient = new RestClient("https://localhost:7227/");

    //context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    string reader = await new StreamReader(context.Request.Body).ReadToEndAsync();
    var stud = Newtonsoft.Json.JsonConvert.DeserializeObject<Student>(reader);
    var userIdent = context.User.Identity;

    stud.UserId = restClient.Get<User>("api/User").
                  FirstOrDefault(u => u.Login == userIdent.Name).Id;

    var checkDupStudentInfo = restClient.Get<Student>("api/Student").
                FirstOrDefault(u => u.UserId == stud.UserId);

    if (stud.FirstName == "" || stud.LastName == "" || stud.PhoneNumber == "")
    {
        await context.Response.WriteAsync(jsonMessage(new { message = "Не все поля заполнены!" }));
        return;
    }

    if (checkDupStudentInfo == null)
    {
        restClient.Post("api/Student", stud);
    }
    else
    {
        stud.Id = checkDupStudentInfo.Id;
        restClient.Put("api/Student", stud);
    }


    if (restClient.IsOk())
    {
        await context.Response.WriteAsync(jsonMessage(new { message = "Ok" }));
        return;
    }

    await context.Response.WriteAsync(jsonMessage($"Failed to add card user data"));
});


app.MapGet("CardStudent", async (HttpContext context) =>
{
    var restClient = new RestClient("https://localhost:7227/");
    var userIdent = context.User.Identity;
    
    var userId = restClient.Get<User>("api/User").
                  FirstOrDefault(u => u.Login == userIdent.Name).Id;

    var studCard = restClient.Get<Student>("api/Student").
                    FirstOrDefault(s => s.UserId == userId);

    var test = jsonMessage(studCard);
    if (studCard != null)
    {
        await context.Response.WriteAsync(jsonMessage(new { find = "Ok", stud = studCard } ));
        return;
    }

    await context.Response.WriteAsync(jsonMessage(new { find = "Not" }));
});




app.MapPost("Faculty", async (HttpContext context) =>
{
    var restClient = new RestClient("https://localhost:7227/");

    string reader = await new StreamReader(context.Request.Body).ReadToEndAsync();
    dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(reader);

    var availableFaculty = restClient.Get<Faculty>("api/Faculty").
                    Where(f => f.UniversityId == (int)json["UniversityId"]).ToList();

    if (availableFaculty != null)
    {
        await context.Response.WriteAsync(jsonMessage(availableFaculty));
        return;
    }

    await context.Response.WriteAsync(jsonMessage(new { fac = "Empty" }));
});


app.MapPost("Application", async (HttpContext context) =>
{
    var restClient = new RestClient("https://localhost:7227/");

    string reader = await new StreamReader(context.Request.Body).ReadToEndAsync();
    dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(reader);

    var userIdent = context.User.Identity;
    var userId = restClient.Get<User>("api/User").
                  FirstOrDefault(u => u.Login == userIdent.Name).Id;

    var studentId = restClient.Get<Student>("api/Student").
              FirstOrDefault(u => u.UserId == userId).Id;

    Application application = new Application()
    {
        ProgramId = (int)json["ProgramId"],
        StudentId = studentId,
        ApplicationDate = DateTime.Now,
    };

    restClient.Post("api/Application", application);

    if (restClient.IsOk())
    {
        var applicationId = restClient.Get<Application>("api/Application").
          FirstOrDefault(a => a.ApplicationDate == application.ApplicationDate).Id;

        ApplicationStatusHistory applicationStatusHistory = new ApplicationStatusHistory()
        {
            ApplicationId = applicationId,
            StatusDate = DateTime.Now,
            ApplicationStatusId = 1
        };

        restClient.Post("api/ApplicationStatusHistory", applicationStatusHistory);

        await context.Response.WriteAsync(jsonMessage(new { message = "Ok" }));
        return;
    }

    await context.Response.WriteAsync(jsonMessage($"Failed to add Application"));
});

app.MapPost("Program", async (HttpContext context) =>
{
    var restClient = new RestClient("https://localhost:7227/");

    string reader = await new StreamReader(context.Request.Body).ReadToEndAsync();
    dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(reader);

    var availableProgram = restClient.Get<Programa>("api/Programa").
                    Where(p => p.FacultyId == (int)json["FacultyId"]).ToList();

    if (availableProgram != null)
    {
        await context.Response.WriteAsync(jsonMessage(availableProgram));
        return;
    }

    await context.Response.WriteAsync(jsonMessage(new { fac = "Empty" }));
});


app.MapPost("Logout", async (HttpContext context) =>
{
    context.Session.SetString("tokenKey", "");
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    await context.Response.WriteAsync(jsonMessage(
        new { message = "Ok", localSite = "https://" + context.Request.Host.ToString() + "/"}
        ));
});


app.Run();
