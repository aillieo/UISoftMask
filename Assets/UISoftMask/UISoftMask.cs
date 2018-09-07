using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AillieoUtils
{
    [ExecuteInEditMode]
    public class UISoftMask : UIBehaviour
    {

        const string maskedShaderName = "AillieoUtils/UISoftMask";

        public Texture2D alphaTexture;


        protected UISoftMask()
        { }

        RectTransform m_RectTransform;
        RectTransform rectTransform
        {
            get { return m_RectTransform ?? (m_RectTransform = GetComponent<RectTransform>()); }
        }

        Vector4 softMaskRect
        {
            get
            {
                Rect r = rectTransform.rect;
                Vector2 pivot = rectTransform.pivot;
                return new Vector4(r.width,r.height,pivot.x,pivot.y);
            }
        }

        Material m_MaskedMaterial;
        Material maskedMaterial
        {
            get
            {
                if (m_MaskedMaterial == null)
                {
                    m_MaskedMaterial = new Material(Shader.Find(maskedShaderName));
                    UpdateAlphaTexture();
                }
                return m_MaskedMaterial;
            }
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            PerformMaskAction();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ResetMaskTargets();

        }


        void UpdateAlphaTexture()
        {
            maskedMaterial.SetTexture("_SoftMaskTex", alphaTexture);
        }
        void UpdateTransformInfo()
        {
            maskedMaterial.SetVector("_SoftMaskRect", softMaskRect);
            maskedMaterial.SetMatrix("_SoftMaskTrans", transform.worldToLocalMatrix);
        }


#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();

            if (!IsActive())
            {
                return;
            }

            UpdateAlphaTexture();
            UpdateTransformInfo();
            PerformMaskAction();
        }



        public bool drawGizmosForDebugging = false;
        Vector3[] fourCorners = new Vector3[4];

        private void OnDrawGizmos()
        {
            if(drawGizmosForDebugging)
            {
                Gizmos.color = Color.yellow;
                rectTransform.GetWorldCorners(fourCorners);
                Rect r = new Rect();
                r.x = fourCorners[0].x;
                r.y = fourCorners[1].y;
                r.width = fourCorners[2].x - fourCorners[1].x;
                r.height = fourCorners[0].y - fourCorners[1].y;
                Gizmos.DrawGUITexture(r, alphaTexture);
            }
        }


#endif




        void PerformMaskAction()
        {
            var targets = GetComponentsInChildren<MaskableGraphic>();
            foreach (var smt in targets)
            {
                smt.material = maskedMaterial;
            }
        }


        void ResetMaskTargets()
        {
            var targets = GetComponentsInChildren<MaskableGraphic>();
            foreach (var smt in targets)
            {
                smt.material = null;
            }
        }


        void Update()
        {
            if(transform.hasChanged)
            {
                UpdateTransformInfo();
                transform.hasChanged = false;
            }
        }


    }
}