using CoTEC_Server.DBModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoTEC_Server.Logic
{
    public static class TemplateGenerator
    {
        public static string GetHTMLString(List<PatientsReport> patients)
        {
            

            var sb = new StringBuilder();
            sb.Append(@"
                        <html>
                            <head>
                            </head>
                            <body>
                                <div class='header'><h1>Patients by country report</h1></div>
                                <table align='center'>
                                    <tr>
                                        <th>Country</th>
                                        <th>Infected</th>
                                        <th>Dead</th>
                                        <th>Recovered</th>
                                        <th>Active</th>
                                    </tr>");

            foreach (var emp in patients)
            {
                sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                    <td>{2}</td>
                                    <td>{3}</td>
                                    <td>{4}</td>
                                  </tr>", emp.Country, emp.Infected, emp.Dead, emp.Cured, emp.Active);
            }

            sb.Append(@"
                                </table>
                            </body>
                        </html>");

            return sb.ToString();
        }

        public static string GetHTMLString(List<CasesAndDeaths> values)
        {


            var sb = new StringBuilder();
            sb.AppendFormat(@"
                        <html>
                            <head>
                            </head>
                            <body>
                                <div class='header'><h1>Patients by country report</h1></div>
                                <table align='center'>
                                    <tr>
                                        <th>Country</th>
                                        <th>Type</th>
                                        <th>{0}</th>
                                        <th>{1}</th>
                                        <th>{2}</th>
                                        <th>{3}</th>
                                        <th>{4}</th>
                                        <th>{5}</th>
                                        <th>{6}</th>
                                    </tr>"
                                        , DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd")
                                        , DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd")
                                        , DateTime.Now.AddDays(-4).ToString("yyyy-MM-dd")
                                        , DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd")
                                        , DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd")
                                        , DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")
                                        , DateTime.Now.ToString("yyyy-MM-dd"));

            foreach (var emp in values)
            {
                sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                    <td>{2}</td>
                                    <td>{3}</td>
                                    <td>{4}</td>
                                    <td>{5}</td>
                                    <td>{6}</td>
                                    <td>{7}</td>
                                    <td>{8}</td>
                                  </tr>"
                                    , emp.Country
                                    , emp.Type
                                    , emp.Fecha1
                                    , emp.Fecha2
                                    , emp.Fecha3
                                    , emp.Fecha4
                                    , emp.Fecha5
                                    , emp.Fecha6
                                    , emp.Fecha7);
            }

            sb.Append(@"
                                </table>
                            </body>
                        </html>");

            return sb.ToString();
        }
    }
}
