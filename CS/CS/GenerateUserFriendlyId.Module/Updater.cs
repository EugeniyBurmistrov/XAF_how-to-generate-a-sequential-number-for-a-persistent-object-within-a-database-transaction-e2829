using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using GenerateUserFriendlyId.Module.Utils;

namespace GenerateUserFriendlyId.Module {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            //Dennis: It is necessary to manually initialize SequenceGenerator when database is updated via the DBUpdater tool.
            SequenceGeneratorInitializer.Initialize();
            //Dennis: It is necessary to register sequences for persistent types used in your application.
            SequenceGenerator.RegisterSequences(XafTypesInfo.Instance.PersistentTypes);
        }
    }
}