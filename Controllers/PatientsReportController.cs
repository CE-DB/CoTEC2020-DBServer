using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoTEC_Server.DBModels;
using CoTEC_Server.Logic;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoTEC_Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PatientsReportController : ControllerBase
    {

        private IConverter _converter;
        private CoTEC_DBContext dbcontext;

        public PatientsReportController(IConverter converter, CoTEC_DBContext dbcontext)
        {
            _converter = converter;
            this.dbcontext = dbcontext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List < PatientsReport > patients = dbcontext.PatientsReport
                .FromSqlRaw("EXECUTE user_public.patients_by_country").ToList();

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "Patients report"
                //Out = @"C:\Users\aleja\Employee_Report.pdf"
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = TemplateGenerator.GetHTMLString(patients),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            var file = _converter.Convert(pdf);
            return File(file, "application/pdf");
        }

        
    }
}
