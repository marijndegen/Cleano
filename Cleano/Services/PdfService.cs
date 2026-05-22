using System;
using System.Collections.Generic;
using System.Linq;
using Cleano.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Cleano.Services
{
    public class PdfService
    {
        public void GeneratePdf(List<TaskGroup> taskGroups, string filePath)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text("Clearo - Cleaning Checklist")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(col =>
                        {
                            col.Item().Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}").FontSize(10);
                            col.Item().PaddingTop(0.5f, Unit.Centimetre);

                            foreach (var group in taskGroups)
                            {
                                var selectedTasks = group.Tasks.Where(t => t.IsSelected).ToList();
                                if (selectedTasks.Any())
                                {
                                    col.Item().PaddingTop(0.5f, Unit.Centimetre).Text(group.Name).SemiBold().FontSize(14);

                                    foreach (var task in selectedTasks.OrderBy(t => t.NextDueDate))
                                    {
                                        col.Item().PaddingLeft(0.5f, Unit.Centimetre).PaddingTop(0.2f, Unit.Centimetre).Row(row =>
                                        {
                                            row.AutoItem().Width(15).Height(15).Border(1).PaddingRight(0.3f, Unit.Centimetre);
                                            row.RelativeItem().Text(text =>
                                            {
                                                text.Span(task.Name);
                                                text.Span($" ({task.Frequency})").FontSize(9).FontColor(Colors.Grey.Darken2);
                                                if (task.IsOverdue)
                                                {
                                                    text.Span(" - OVERDUE").FontColor(Colors.Red.Medium).SemiBold();
                                                }
                                            });
                                        });
                                    }
                                }
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            })
            .GeneratePdf(filePath);
        }
    }
}
