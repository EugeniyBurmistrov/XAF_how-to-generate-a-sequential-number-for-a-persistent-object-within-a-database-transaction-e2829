using DevExpress.ExpressApp;
using GenerateUserFriendlyId.Module.BO;
using GenerateUserFriendlyId.Module.Utils;

namespace GenerateUserFriendlyId.Module {
    public sealed partial class GenerateUserFriendlyIdModule : ModuleBase {
        public GenerateUserFriendlyIdModule() {
            InitializeComponent();
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);
            XafTypesInfo.Instance.RegisterEntity("Document", typeof(IDocument), typeof(BasePersistentObject));
            SequenceGeneratorInitializer.Register(application);
        }
    }
}