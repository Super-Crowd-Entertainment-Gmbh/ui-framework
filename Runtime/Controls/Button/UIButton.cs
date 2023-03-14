using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rehawk.UIFramework
{
    public class UIButton : UIButtonBase, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private bool isInteractable = true;

        private Button button;
        
        public override bool IsInteractable
        {
            get
            {
                if (button)
                {
                    return button.interactable;
                }

                return isInteractable;
            }
            set
            {
                if (button)
                {
                    button.interactable = value;
                }

                isInteractable = value;
                OnPropertyChanged();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            button = GetComponent<Button>();
            
            if (button)
            {
                isInteractable = button.interactable;
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (IsInteractable)
            {
                InvokeCommand(HOVER_BEGIN_COMMAND);
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (IsInteractable)
            {
                InvokeCommand(HOVER_END_COMMAND);
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (IsInteractable)
            {
                InvokeCommand(CLICK_COMMAND);
            }
        }
    }
}