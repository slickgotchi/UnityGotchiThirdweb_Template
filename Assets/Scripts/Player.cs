using UnityEngine;

namespace GotchiHub
{
    public class Player : MonoBehaviour
    {
        public float speed = 5f;
        public SpriteRenderer Body;

        private Sprite m_back;
        private Sprite m_front;
        private Sprite m_left;
        private Sprite m_right;

        private void Start()
        {
            if (GotchiDataManager.Instance != null)
            {
                Debug.Log("Listening to gotchi data manager");
                GotchiDataManager.Instance.onSelectedGotchi += OnSelectedGotchiChanged;
            }
        }

        private void OnDisable()
        {
            if (GotchiDataManager.Instance != null)
            {
                GotchiDataManager.Instance.onSelectedGotchi -= OnSelectedGotchiChanged;
            }
        }

        private void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0.0f);
            transform.Translate(movement * speed * Time.deltaTime, Space.World);

            UpdateSpriteDirection(movement);
        }

        private void UpdateSpriteDirection(Vector3 movement)
        {
            if (GotchiDataManager.Instance.gotchiData.Count <= 0) return;

            if (movement.magnitude > 0)
            {
                if (movement.y > 0)
                {
                    Body.sprite = m_back;
                }
                else if (movement.y < 0)
                {
                    Body.sprite = m_front;
                }
                else if (movement.x > 0)
                {
                    Body.sprite = m_right;
                }
                else if (movement.x < 0)
                {
                    Body.sprite = m_left;
                }
            }
        }

        private void OnSelectedGotchiChanged(int newGotchiId)
        {
            Debug.Log("Selected Gotchi ID changed to: " + newGotchiId);
            var gotchiSvgs = GotchiDataManager.Instance.GetGotchiSvgsById(newGotchiId);

            // Cache the sprites
            m_front = GetSpriteFromSvgString(gotchiSvgs.svg);
            m_back = GetSpriteFromSvgString(gotchiSvgs.back);
            m_left = GetSpriteFromSvgString(gotchiSvgs.left);
            m_right = GetSpriteFromSvgString(gotchiSvgs.right);

            // Set initial sprite
            Body.sprite = m_front;
        }

        private Sprite GetSpriteFromSvgString(string svgString)
        {
            // Convert SVG string to a Sprite
            return SvgLoader.CreateSvgSprite(GotchiDataManager.Instance.stylingGame.CustomizeSVG(svgString), Vector2.zero);
        }
    }
}