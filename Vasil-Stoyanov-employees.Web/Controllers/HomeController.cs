namespace Vasil_Stoyanov_employees.Web.Controllers
{
    using System.IO;
    using System.Diagnostics;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Models;
    using Services;

    using static Common.DataConstants;
    using static Common.DataConstants.ErrorMessages;

    public class HomeController : Controller
    {
        private readonly ICalculatorService _calculatorService;

        public HomeController(ICalculatorService calculatorService)
            => this._calculatorService = calculatorService;

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(FileFormModel fileModel)
        {
            if (!ModelState.IsValid)
            {
                this.TempData[InvalidFileExtensionKey] = InvalidExtensionType;

                return this.RedirectToAction(nameof(this.Index));
            }

            var file = fileModel.File;
            var fileName = file.FileName;
            var fileExtension = Path.GetExtension(fileName).Replace(".", "");

            if (fileExtension != "csv")
            {
                this.TempData[InvalidFileExtensionKey] = InvalidExtensionType;

                return this.RedirectToAction(nameof(this.Index));
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileNameWithPath = Path.Combine(path, fileName);

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            this.TempData[FineNameKey] = fileName;

            return this.RedirectToAction(nameof(this.Result));
        }

        public IActionResult Result()
        {
            var fileName = this.TempData[FineNameKey].ToString();
            var bestPairModel = this._calculatorService.GetTheBestPair(fileName);

            return View(bestPairModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
