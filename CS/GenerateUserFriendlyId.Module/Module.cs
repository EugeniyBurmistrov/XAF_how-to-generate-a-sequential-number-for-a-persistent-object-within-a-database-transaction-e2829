using System;
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
                XafTypesInfo.Instance.AddEntityToGenerate("Document", typeof(IDocument));
            }
        }
    }
}
