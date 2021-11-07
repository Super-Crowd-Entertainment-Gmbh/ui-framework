using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public interface IView : IPanel {}
    
    public interface IView<TContext> : IView, IElement<TContext> {}

    public abstract class View : Panel, IView
    {
        [BoxGroup("Visibility", false)]
        [DisableInPlayMode]
        [OdinSerialize] private IUITweener tweener;
        
        [BoxGroup("Visibility", false)]
        [DisableInPlayMode]
        [Indent, HideIf("@this.tweener != null")]
        [SerializeField] private Canvas canvas;

        [BoxGroup("Visibility", false)]
        [DisableInPlayMode]
        [Indent, HideIf("@this.tweener != null || this.canvas != null")]
        [SerializeField] private RectTransform root;
        
        [BoxGroup("Visibility", false)]
        [DisableInPlayMode]
        [SerializeField] private bool hiddenByDefault;
        
        protected Canvas Canvas
        {
            get { return canvas; }
        }
        
        protected RectTransform Root
        {
            get { return root; }
        }
        
        public override bool IsVisible
        {
            get
            {
                if (tweener != null)
                    return tweener.IsVisible;

                if (canvas)
                    return canvas.enabled;

                if (root)
                    return root.gameObject.activeSelf;
                
                return false;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (hiddenByDefault)
            {
                Hide(0);
            }
            else
            {
                Show();
            }
        }
        
        protected override void OnShow(float duration = 0)
        {
            base.OnShow(duration);
            
            if (tweener == null)
            {
                if (Canvas)
                {
                    Canvas.enabled = true;
                }
                else if (Root)
                {
                    root.gameObject.SetActive(true);
                }
            }
            else
            {
                tweener.Show(duration);
            }
        }

        protected override void OnHide(float duration = 0)
        {
            base.OnHide(duration);
            
            if (tweener == null)
            {
                if (Canvas)
                {
                    Canvas.enabled = false;
                }
                else if (Root)
                {
                    root.gameObject.SetActive(false);
                }
            }
            else
            {
                tweener.Hide(duration);
            }
        }
    }

    public abstract class View<TContext> : View, IView<TContext>
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
        /// Is called right after the context has changed but before the view gets dirty.
        /// Example: Use it to subscribe to events of the new context.
        /// </summary>
        protected virtual void AfterContextChanged() {}
    }
}