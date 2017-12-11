using BitcoinBetting.iOS.ExportRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer (typeof(ViewCell), typeof(CustomAllViewCellRendereriOs))]
namespace BitcoinBetting.iOS.ExportRenderer
{
    public class CustomAllViewCellRendereriOs : ViewCellRenderer
    {
        public override UIKit.UITableViewCell GetCell(Cell item, UIKit.UITableViewCell reusableCell, UIKit.UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);
            
            if (cell != null)
                cell.SelectionStyle = UIKit.UITableViewCellSelectionStyle.None;
            
            return cell;
        }
    }
}