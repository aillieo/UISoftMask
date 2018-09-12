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

        public GameObject[] managedMaskingTarget;


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


        public bool MaskEnabled() { return IsActive() && alphaTexture != null; }


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
        }



        public bool drawGizmosForDebugging = false;
        Vector3[] fourCorners = new Vector3[4];

        private void OnDrawGizmos()
        {
            if(drawGizmosForDebugging && MaskEnabled())
            {
                Gizmos.color = Color.yellow;
                rectTransform.GetWorldCorners(fourCorners);
                Rect r = new Rect
                {
                    x = fourCorners[0].x,
                    y = fourCorners[1].y,
                    width = fourCorners[2].x - fourCorners[1].x,
                    height = fourCorners[0].y - fourCorners[1].y
                };
                Gizmos.DrawGUITexture(r, alphaTexture);
                for (int i = 0; i < 4; i++)
                {
                    Gizmos.DrawLine(fourCorners[i], fourCorners[(i + 1) % 4]);
                }
            }
        }


#endif


        protected override void OnEnable()
        {
            base.OnEnable();
            if(MaskEnabled())
            {
                MaskAllChildren();
                MaskAllManaged();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            ResetAllChildren();
            ResetAllManaged();
        }


        public void MaskOneGameObject(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }
            var mgs = obj.GetComponentsInChildren<MaskableGraphic>();
            foreach (var mg in mgs)
            {
                Image img = mg as Image;
                if (img)
                {
                    if (img.sprite.associatedAlphaSplitTexture != null)
                    {
                        mg.material = maskedMaterialETC1;
                        continue;
                    }
                }
                mg.material = maskedMaterial;
            }
        }

        public void ResetOneGameObject(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }
            var mgs = obj.GetComponentsInChildren<MaskableGraphic>();
            foreach (var mg in mgs)
            {
                mg.material = null;
            }
        }


        public void MaskAllChildren()
        {
            foreach (Transform t in transform)
            {
                MaskOneGameObject(t.gameObject);
            }
        }


        public void ResetAllChildren()
        {
            foreach (Transform t in transform)
            {
                ResetOneGameObject(t.gameObject);
            }
        }

        public void MaskAllManaged()
        {
            if(managedMaskingTarget == null)
            {
                return;
            }
            foreach(var obj in managedMaskingTarget)
            {
                MaskOneGameObject(obj);
            }
        }

        public void ResetAllManaged()
        {
            if (managedMaskingTarget == null)
            {
                return;
            }
            foreach (var obj in managedMaskingTarget)
            {
                ResetOneGameObject(obj);
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