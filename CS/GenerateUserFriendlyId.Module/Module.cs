using DevExpress.ExpressApp;
using GenerateUserFriendlyId.Module.BO;

namespace GenerateUserFriendlyId.Module {
    public sealed partial class GenerateUserFriendlyIdModule : ModuleBase {
        public GenerateUserFriendlyIdModule() {
            InitializeComponent();
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (!XafTypesInfo.IsInitialized) {
                //Dennis: Starting from version 11.1, set the last parameter to True.
                //XafTypesInfo.Instance.AddEntityToGenerate("Document", typeof(IDocument), typeof(BasePersistentObject), true);
                XafTypesInfo.Instance.AddEntityToGenerate("Document", typeof(IDocument), typeof(BasePersistentObject));
            }
        }
    }
}
