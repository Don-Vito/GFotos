using System.Windows;
using System.Windows.Input;

namespace GFotos.View
{
    /// <summary>
    /// Interaction logic for GroupDirectoresView.xaml
    /// </summary>
    public partial class GroupDirectoresView
    {
        public static readonly DependencyProperty GlobalPreferCommandProperty = DependencyProperty.Register("GlobalPreferCommand", typeof(ICommand), typeof(GroupDirectoresView), new PropertyMetadata(default(ICommand)));

        public GroupDirectoresView()
        {
            InitializeComponent();            
        }      

        public ICommand GlobalPreferCommand
        {
            get { return (ICommand) GetValue(GlobalPreferCommandProperty); }
            set { SetValue(GlobalPreferCommandProperty, value); }
        }
    }
}
