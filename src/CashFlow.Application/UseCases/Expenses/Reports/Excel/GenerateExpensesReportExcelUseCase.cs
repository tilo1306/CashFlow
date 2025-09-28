using CashFlow.Domain.Enums;
using CashFlow.Domain.Reports;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Services.LoggedUser;
using ClosedXML.Excel;

namespace CashFlow.Application.UseCases.Expenses.Reports.Excel;

public class GenerateExpensesReportExcelUseCase : IGenerateExpensesReportExcelUseCase
{
    private const string CURRENCY_SYMBOL = "R$";
    private readonly IExpensesReadonlyRepository _repository;
    private readonly ILoggedUser _loggedUser;


    public GenerateExpensesReportExcelUseCase(IExpensesReadonlyRepository repository,
      ILoggedUser loggedUser)
    {
        _repository = repository;
        _loggedUser = loggedUser;
    }

    public async Task<byte[]> Execute(DateOnly month)
    {
        var loggedUser = await _loggedUser.Get();
      
        var expenses = await _repository.FilterByMonth(loggedUser, month);

        if (expenses.Count == 0)
        {
            return [];
        }

        using var workbook = new XLWorkbook();

        workbook.Author = loggedUser.Name;
        workbook.Style.Font.FontSize = 12;
        workbook.Style.Font.FontName = "Times New Roman";

        var workSheet = workbook.Worksheets.Add(month.ToString("Y"));

        InsertHeader(workSheet);

        var raw = 2;

        foreach (var expense in expenses)
        {
            workSheet.Cell($"A{raw}").Value = expense.Title;
            workSheet.Cell($"B{raw}").Value = expense.Date;
            workSheet.Cell($"C{raw}").Value = ConvertPaymentType(expense.PaymentType);
            workSheet.Cell($"D{raw}").Value = expense.Amount;
            workSheet.Cell($"D{raw}").Style.NumberFormat.Format = $"-{CURRENCY_SYMBOL} #,##0.00";
            workSheet.Cell($"E{raw}").Value = expense.Description;

            raw++;
        }

        workSheet.Columns().AdjustToContents();

        var file = new MemoryStream();

        workbook.SaveAs(file);

        return file.ToArray();
    }

    private string ConvertPaymentType(PaymentType payment)
    {
        return payment switch
        {
            PaymentType.Cash => "Dinheiro",
            PaymentType.CreditCard => "Cartão de Credito",
            PaymentType.DebitCard => "Cartão de Debito",
            PaymentType.EletronicTransfer => "Transferencia Bancaria",
            _ => string.Empty,
        };
    }

    private void InsertHeader(IXLWorksheet workSheet)
    {
        workSheet.Cell("A1").Value = ResourceReportGenerationMessages.TITLE;
        workSheet.Cell("B1").Value = ResourceReportGenerationMessages.DATE;
        workSheet.Cell("C1").Value = ResourceReportGenerationMessages.PAYMENT_TYPE;
        workSheet.Cell("D1").Value = ResourceReportGenerationMessages.AMOUNT;
        workSheet.Cell("E1").Value = ResourceReportGenerationMessages.DESCRIPTION;

        workSheet.Cell("A1:E1").Style.Font.Bold = true;
        workSheet.Cell("A1:E1").Style.Fill.BackgroundColor = XLColor.FromHtml("#F5C2B6");

        workSheet.Cell("A1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        workSheet.Cell("B1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        workSheet.Cell("C1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        workSheet.Cell("E1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        workSheet.Cell("D1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
    }
}
