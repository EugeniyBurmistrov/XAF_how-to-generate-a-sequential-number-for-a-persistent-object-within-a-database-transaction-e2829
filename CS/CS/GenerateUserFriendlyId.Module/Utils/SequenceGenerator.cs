using System;
using DevExpress.Xpo;
using System.Threading;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Xpo.Metadata;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using DevExpress.Xpo.DB.Exceptions;

namespace GenerateUserFriendlyId.Module.Utils {
    //This class is used to generate sequential numbers for persistent objects.
    //Use the GetNextSequence method to get the next number and the Accept method, to save these changes to the database.
    public class SequenceGenerator : IDisposable {
        public const int MaxGenerationAttemptsCount = 10;
        public const int MinGenerationAttemptsDelay = 100;
        private static IDataLayer defaultDataLayer;
        private ExplicitUnitOfWork euow;
        private Sequence seq;
        public SequenceGenerator() {
            int count = MaxGenerationAttemptsCount;
            while (true) {
                try {
                    euow = new ExplicitUnitOfWork(DefaultDataLayer);
                    XPCollection<Sequence> sequences = new XPCollection<Sequence>(euow);
                    foreach (Sequence seq in sequences)
                        seq.Save();
                    euow.FlushChanges();
                    break;
                } catch (LockingException) {
                    Close();
                    count--;
                    if (count <= 0)
                        throw;
                    Thread.Sleep(MinGenerationAttemptsDelay * count);
                }
            }
        }
        public void Accept() {
            euow.CommitChanges();
        }
        public long GetNextSequence(object theObject) {
            if (theObject == null)
                throw new ArgumentNullException("theObject");
            return GetNextSequence(XafTypesInfo.Instance.FindTypeInfo(theObject.GetType()));
        }
        public long GetNextSequence(ITypeInfo typeInfo) {
            if (typeInfo == null)
                throw new ArgumentNullException("typeInfo");
            return GetNextSequence(XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(typeInfo.Type));
        }
        public long GetNextSequence(XPClassInfo classInfo) {
            if (classInfo == null)
                throw new ArgumentNullException("classInfo");
            seq = euow.GetObjectByKey<Sequence>(classInfo.FullName, true);
            if (seq == null) {
                throw new InvalidOperationException(string.Format("Sequence for the {0} type was not found.", classInfo.FullName));
            }
            long nextSequence = seq.NextSequence;
            seq.NextSequence++;
            euow.FlushChanges();
            return nextSequence;
        }
        public void Close() {
            if (euow != null) {
                euow.Dispose();
                euow = null;
            }
        }
        public void Dispose() {
            Close();
        }
        public static IDataLayer DefaultDataLayer {
            get {
                if (defaultDataLayer == null)
                    throw new ArgumentNullException("DefaultDataLayer");
                return defaultDataLayer;
            }
            set { defaultDataLayer = value; }
        }
        public static void RegisterSequences(IEnumerable<ITypeInfo> persistentTypes) {
            if (persistentTypes != null)
                using (UnitOfWork uow = new UnitOfWork(DefaultDataLayer)) {
                    XPCollection<Sequence> sequenceList = new XPCollection<Sequence>(uow);
                    Dictionary<string, bool> typeToExistsMap = new Dictionary<string, bool>();
                    foreach (Sequence seq in sequenceList) {
                        typeToExistsMap[seq.TypeName] = true;
                    }
                    foreach (ITypeInfo typeInfo in persistentTypes) {
                        if (typeToExistsMap.ContainsKey(typeInfo.FullName)) continue;
                        Sequence seq = new Sequence(uow);
                        seq.TypeName = typeInfo.FullName;
                        seq.NextSequence = 0;
                    }
                    uow.CommitChanges();
                }
        }
        public static void RegisterSequences(IEnumerable<XPClassInfo> persistentClasses) {
            if (persistentClasses != null)
                using (UnitOfWork uow = new UnitOfWork(DefaultDataLayer)) {
                    XPCollection<Sequence> sequenceList = new XPCollection<Sequence>(uow);
                    Dictionary<string, bool> typeToExistsMap = new Dictionary<string, bool>();
                    foreach (Sequence seq in sequenceList) {
                        typeToExistsMap[seq.TypeName] = true;
                    }
                    foreach (XPClassInfo classInfo in persistentClasses) {
                        if (typeToExistsMap.ContainsKey(classInfo.FullName)) continue;
                        Sequence seq = new Sequence(uow);
                        seq.TypeName = classInfo.FullName;
                        seq.NextSequence = 0;
                    }
                    uow.CommitChanges();
                }
        }
    }
    //This persistent class is used to store last sequential number for persistent objects.
    public class Sequence : XPBaseObject {
        private string typeName;
        private long nextSequence;
        public Sequence(Session session)
            : base(session) {
        }
        [Key]
        //Dennis: The size should be enough to store a full type name. However, you cannot use unlimited size for key columns.
        [Size(1024)]
        public string TypeName {
            get { return typeName; }
            set { SetPropertyValue("TypeName", ref typeName, value); }
        }
        public long NextSequence {
            get { return nextSequence; }
            set { SetPropertyValue("NextSequence", ref nextSequence, value); }
        }
    }
    public interface ISupportSequentialNumber {
        long SequentialNumber { get; set; }
    }
}