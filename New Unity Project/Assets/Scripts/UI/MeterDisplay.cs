using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MeterDisplay : MonoBehaviour
    {
        [SerializeField] private Color _fullColor = Color.green;
        [SerializeField] private Color _halfColor = Color.yellow;
        [SerializeField] private Color _emptyColor = Color.red;

        private Transform _barTransform;
        private Image _barImage;

        public float StartValue { set { DisplayValue = value; SetBarDisplay(value); } }
        public float DisplayValue { private get; set; }

        private void Awake()
        {
            _barTransform = transform.Find("Meter bar");
            _barImage = _barTransform.GetComponent<Image>();
        }

        private void OnEnable()
        {
            SetBarDisplay(0.0f);
            DisplayValue = 0.0f;
        }

        private void SetBarDisplay(float actualValue)
        {
            actualValue = Mathf.Clamp01(actualValue);

            if (_barTransform != null)
            {
                _barTransform.localScale = new Vector3(actualValue, 1.0f, 1.0f);

                float colorModifier = actualValue * 2.0f;

                _barImage.color = colorModifier >= 1.0f
                    ? Color.Lerp(_halfColor, _fullColor, colorModifier - 1.0f)
                    : Color.Lerp(_emptyColor, _halfColor, colorModifier);
            }
        }

        private void FixedUpdate()
        {
            if (Mathf.Abs(_barTransform.localScale.x - DisplayValue) < Time.fixedDeltaTime)
            {
                SetBarDisplay(DisplayValue);
            }
            else
            {
                SetBarDisplay(_barTransform.localScale.x + Time.fixedDeltaTime * Mathf.Sign(DisplayValue - _barTransform.localScale.x));
            }
        }
    }
}