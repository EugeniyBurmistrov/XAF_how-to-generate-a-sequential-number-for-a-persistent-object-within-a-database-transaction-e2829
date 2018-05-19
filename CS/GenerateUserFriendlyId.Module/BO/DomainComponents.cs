using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using GenerateUserFriendlyId.Module.Utils;
using System.ComponentModel;

namespace GenerateUserFriendlyId.Module.BO {
    [DomainComponent]
    public interface IBaseDomainComponent : ISupportSequentialNumber { }

    [DomainLogic(typeof(IBaseDomainComponent))]
    public class BaseDomainComponentDomainLogic {
        private static SequenceGenerator sequenceGenerator;
        public void OnSaving(IBaseDomainComponent theObject, IObjectSpace os) {
            if (os.IsNewObject(theObject))
                GenerateSequence(theObject, os);
        }
        public static void GenerateSequence(IBaseDomainComponent theObject, IObjectSpace os) {
            if (sequenceGenerator == null)
                sequenceGenerator = new SequenceGenerator();
            theObject.SequentialNumber = sequenceGenerator.GetNextSequence(theObject);
            if (!(os is NestedObjectSpace))
                os.Committed += OnCommitted;
        }
        private static void OnCommitted(object sender, EventArgs e) {
            IObjectSpace os = (IObjectSpace)sender;
            if (sequenceGenerator != null) {
                try {
                    sequenceGenerator.Accept();
                } finally {
                    os.Committed -= OnCommitted;
                    sequenceGenerator.Dispose();
                    sequenceGenerator = null;
                }
            }
        }
    }
    [DomainComponent]
    [XafDefaultProperty("Title")]
    [DefaultClassOptions]
    public interface IDocument : IBaseDomainComponent {
        [Calculated("concat('D', ToStr(SequentialNumber))")]
        string DocumentId { get; }
        [RuleRequiredField("IDocument.Title.RuleRequiredField", DefaultContexts.Save)]
        [FieldSize(255)]
        string Title { get; set; }
        [FieldSize(4000)]
        string Text { get; set; }
        [PersistentDc, Browsable(false)]
        new long SequentialNumber { get; set; }
    }
}