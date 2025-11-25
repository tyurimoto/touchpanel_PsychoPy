using System.Windows.Forms;

namespace Compartment
{
    public partial class UserControlPreferencesTab : UserControl
    {
        public UserControlPreferencesTab()
        {
            InitializeComponent();
#if !IGNORE_DOOR_VISIBLE
            checkBoxIgnoreDoorError.Visible = false;
#endif
        }
    }
}
