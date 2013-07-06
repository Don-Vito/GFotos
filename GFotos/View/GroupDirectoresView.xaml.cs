using System.Windows;
using System.Windows.Input;
using GFotos.ViewModel.ImageGrouping;

namespace GFotos.View
{
    /// <summary>
    /// Interaction logic for GroupDirectoresView.xaml
    /// </summary>
    public partial class GroupDirectoresView
    {
        public static readonly DependencyProperty GlobalPreferCommandProperty = DependencyProperty.Register("GlobalPreferCommand", typeof(ICommand), typeof(GroupDirectoresView), new PropertyMetadata(default(ICommand)));
        public static readonly DependencyProperty ImagesGroupProperty = DependencyProperty.Register("ImagesGroup", typeof(RedundantImagesGroup), typeof(GroupDirectoresView), new PropertyMetadata(default(RedundantImagesGroup)));

        public GroupDirectoresView()
        {
            InitializeComponent();
            DataContext = this;
        }      

        public ICommand GlobalPreferCommand
        {
            get { return (ICommand) GetValue(GlobalPreferCommandProperty); }
            set { SetValue(GlobalPreferCommandProperty, value); }
        }

        public RedundantImagesGroup ImagesGroup
        {
            get { return (RedundantImagesGroup)GetValue(ImagesGroupProperty); }
            set { SetValue(ImagesGroupProperty, value); }
        }
    }
}
