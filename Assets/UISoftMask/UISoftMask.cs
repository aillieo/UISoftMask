using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AillieoUtils
{
    [ExecuteInEditMode]
    public class UISoftMask : UIBehaviour
    {

        const string maskedShaderName = "AillieoUtils/UISoftMask";
        const string maskedShaderNameETC1 = "AillieoUtils/UISoftMaskETC1";

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
        Material m_MaskedMaterialETC1;
        Material maskedMaterialETC1
        {
            get
            {
                if (m_MaskedMaterialETC1 == null)
                {
                    m_MaskedMaterialETC1 = new Material(Shader.Find(maskedShaderNameETC1));
                    UpdateAlphaTexture();
                }
                return m_MaskedMaterialETC1;
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
            maskedMaterialETC1.SetTexture("_SoftMaskTex", alphaTexture);
        }

        void UpdateTransformInfo()
        {
            maskedMaterial.SetVector("_SoftMaskRect", softMaskRect);
            maskedMaterial.SetMatrix("_SoftMaskTrans", transform.worldToLocalMatrix);
            maskedMaterialETC1.SetVector("_SoftMaskRect", softMaskRect);
            maskedMaterialETC1.SetMatrix("_SoftMaskTrans", transform.worldToLocalMatrix);
        }


        public virtual bool MaskEnabled() { return IsActive() && alphaTexture != null; }


#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();

            if (!MaskEnabled())
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
            if(drawGizmosForDebugging && MaskEnabled())
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

        private void OnTransformChildrenChanged()
        {
            PerformMaskAction();
        }


        public void PerformMaskAction()
        {
            var mgs = GetComponentsInChildren<MaskableGraphic>();
            foreach (var mg in mgs)
            {
                Image img = mg as Image;
                if (img)
                {
                    if(img.sprite.associatedAlphaSplitTexture != null)
                    {
                        mg.material = maskedMaterialETC1;
                        continue;
                    }
                }
                mg.material = maskedMaterial;
            }
        }


        public void ResetMaskTargets()
        {
            var mgs = GetComponentsInChildren<MaskableGraphic>();
            foreach (var mg in mgs)
            {
                mg.material = null;
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