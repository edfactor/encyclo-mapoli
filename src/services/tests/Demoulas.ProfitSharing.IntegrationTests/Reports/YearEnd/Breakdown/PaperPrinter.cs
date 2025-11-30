using System.Text;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Breakdown;

// Simulates printing on paper.   Mostly this means accepting line after line of test and
// keeping of the current line number.  If we get high enough, we'll need to start a new page.  A new page
// is a new line, form feed, and a new header section.   The header section usually includes the page number and sometimes store number (which changes.)
public class PaperPrinter
{
    private readonly StringBuilder _sb = new();
    private int _lineNumber;
    private short _pageNumber;

    public PaperPrinter(short pageNumber)
    {
        _lineNumber = 0;
        _pageNumber = pageNumber;
    }

    public Func<short, string>? HeaderTemplate { get; set; }

    public void line(string text)
    {
        if (_lineNumber > 56)
        {
            _lineNumber = 0;
            line("\f");
            newLine();
            insertHeader();
        }

        _sb.AppendLine(text);
        _lineNumber++;
    }

    public void insertHeader()
    {
        if (HeaderTemplate == null)
        {
            return;
        }

        string header = HeaderTemplate(_pageNumber);
        _lineNumber = header.Count(ch => ch == '\n');
        _sb.Append(header);
        _pageNumber++;
    }

    public void newLine()
    {
        line("");
    }

    public new string ToString()
    {
        return _sb.ToString();
    }
}
