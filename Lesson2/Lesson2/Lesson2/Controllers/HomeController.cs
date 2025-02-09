using Lesson2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Services;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;

namespace Lesson2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDadataService _dadataService;
        private static List<WebSocket> webSockets = new();

        public HomeController(ILogger<HomeController> logger, IDadataService dadataService)
        {
            _logger = logger;
            _dadataService = dadataService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> List(string inn)
        {
            var result = await _dadataService.GetSuggestionAsync(inn);
            return PartialView(result?.Suggestions);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("/ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                webSockets.Add(webSocket);
                await Echo(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private async Task Echo(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var segment = new ArraySegment<byte>(buffer);
            var receiveResult = await webSocket.ReceiveAsync(
                segment, CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                var str = System.Text.Encoding.Default.GetString(segment);
                int i = str.IndexOf('\0');
                    if (i >= 0) str = str.Substring(0, i);

                string result = "";
                if(str != null && str != String.Empty)
                {
                    var data = await _dadataService.GetSuggestionAsync(str);
                    if ((data?.Suggestions)?.Any() == true)
                        result = await this.RenderViewToStringAsync("List", data.Suggestions);                    
                }
                
                if(result == null || result == string.Empty)
                    result = "NotFound";

                var bytes = System.Text.Encoding.Default.GetBytes(result);

                foreach (var socket in webSockets)
                {
                    if (socket.State == WebSocketState.Open)
                    {
                        await socket.SendAsync(
                            new ArraySegment<byte>(bytes, 0, bytes.Length),
                            WebSocketMessageType.Text,
                            receiveResult.EndOfMessage,
                            CancellationToken.None);
                    }
                }

                segment = new ArraySegment<byte>(buffer);
                receiveResult = await webSocket.ReceiveAsync(
                    segment, CancellationToken.None);
            }

            webSockets.Remove(webSocket);

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }
    }

    public static class ControllerExtensions
    {
        /// <summary>
        /// Render a partial view to string.
        /// </summary>
        public static async Task<string> RenderViewToStringAsync(this Controller controller, string viewNamePath, object model)
        {
            if (string.IsNullOrEmpty(viewNamePath))
                viewNamePath = controller.ControllerContext.ActionDescriptor.ActionName;

            controller.ViewData.Model = model;

            using (StringWriter writer = new StringWriter())
            {
                try
                {
                    var view = FindView(controller, viewNamePath);

                    ViewContext viewContext = new ViewContext(
                        controller.ControllerContext,
                        view,
                        controller.ViewData,
                        controller.TempData,
                        writer,
                        new HtmlHelperOptions()
                    );

                    await view.RenderAsync(viewContext);

                    return writer.GetStringBuilder().ToString();
                }
                catch (Exception exc)
                {
                    return $"Failed - {exc.Message}";
                }
            }
        }

        private static IView FindView(Controller controller, string viewNamePath)
        {
            IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

            ViewEngineResult viewResult = null;

            if (viewNamePath.EndsWith(".cshtml"))
                viewResult = viewEngine.GetView(viewNamePath, viewNamePath, false);
            else
                viewResult = viewEngine.FindView(controller.ControllerContext, viewNamePath, false);

            if (!viewResult.Success)
            {
                var endPointDisplay = controller.HttpContext.GetEndpoint().DisplayName;

                if (endPointDisplay.Contains(".Areas."))
                {
                    //search in Areas
                    var areaName = endPointDisplay.Substring(endPointDisplay.IndexOf(".Areas.") + ".Areas.".Length);
                    areaName = areaName.Substring(0, areaName.IndexOf(".Controllers."));

                    viewNamePath = $"~/Areas/{areaName}/views/{controller.HttpContext.Request.RouteValues["controller"]}/{controller.HttpContext.Request.RouteValues["action"]}.cshtml";

                    viewResult = viewEngine.GetView(viewNamePath, viewNamePath, false);
                }

                if (!viewResult.Success)
                    throw new Exception($"A view with the name '{viewNamePath}' could not be found");

            }

            return viewResult.View;
        }

    }
}