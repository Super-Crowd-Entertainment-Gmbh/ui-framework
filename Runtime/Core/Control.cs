using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public interface IControl : IElement {}
    
    public interface IControl<TContext> : IControl, IElement<TContext> {}

    public abstract class Control : Element, IControl {}

    public abstract class Control<TContext> : Control, IControl<TContext>
    {
        [HorizontalGroup("Context"), LabelText("Context")]
        [SerializeField] private bool useStaticContext;
        [HorizontalGroup("Context"), HideLabel, EnableIf("@this.useStaticContext")]
        [OdinSerialize] private TContext staticContext;
        
        public TContext Context { get; private set; }

        public bool HasContext
        {
            get { return !Equals(Context, default(TContext)); }
        }

        protected override void Start()
        {
            base.Start();

            if (useStaticContext)
            {
                SetContext(staticContext);
            }
        }

        public void SetContext(TContext context)
        {
            BeforeContextChanged();
            Context = context;
            AfterContextChanged();
            SetDirty();
        }
        
        /// <summary>
        /// Is called right before the context is changed.
        /// Example: Use it to unsubscribe from events of the previous context.
        /// </summary>
        protected virtual void BeforeContextChanged() {}
        
        /// <summary>
        /// Is called right after the context has changed but before the control gets dirty.
        /// Example: Use it to subscribe to events of the new context.
        /// </summary>
        protected virtual void AfterContextChanged() {}
    }
}