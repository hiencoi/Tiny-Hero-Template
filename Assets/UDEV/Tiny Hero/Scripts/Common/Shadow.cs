using UnityEngine;
using System.Collections;

namespace UDEV.TinyHero {
	public class Shadow : MonoBehaviour {
        [LayerList]
        public int groundLayer;
		public GameObject shadowedObject;
		public Vector3    offset      = Vector3.zero;
        public Vector3 checkOffset = Vector3.zero;
		public float      maxDistance = 5;
		public float      scaleXTo    = 1.5f;
        public float      scaleYTo    = 1.5f;
        public bool       fadeOut     = true;

		Renderer   renderCom;
		Collider2D col2D;

		void Awake() {
			renderCom = GetComponent<Renderer>();
		}

		void Start() {
			col2D = shadowedObject.GetComponentInChildren<Collider2D>();
            transform.localScale = new Vector3(scaleXTo, scaleYTo, 0);
        }

		void LateUpdate () {
			Vector3 pos = shadowedObject.transform.position;
			RaycastHit2D[] hits = Physics2D.RaycastAll(pos + checkOffset, new Vector2(0, -1), maxDistance, 1 << groundLayer);
			RaycastHit2D   hit  = new RaycastHit2D();
			float          closest  = maxDistance;
			bool           found    = false;

			for (int i = 0; i < hits.Length; i++) {
				float dist = ((Vector2)pos - hits[i].point).magnitude / maxDistance;
				if (hits[i].collider != col2D && dist <= closest) {
					hit     = hits[i];
					closest = dist;
					found   = true;
				}
			}

			if (found) {
                var xd = shadowedObject.transform.localScale.x / Mathf.Abs(shadowedObject.transform.localScale.x);
                var zd = shadowedObject.transform.localScale.z / Mathf.Abs(shadowedObject.transform.localScale.z);

                transform.position = (Vector3)hit.point + new Vector3(offset.x * xd * zd, offset.y, offset.z);
				FitGround(hit.normal);
				Modifiers(closest);
				renderCom.enabled = true;
			} else {
				renderCom.enabled = false;
			}
		}

		void Modifiers(float aPercent) {
			if (fadeOut) {
				Color c = renderCom.material.color;
				c.a = 1-aPercent*aPercent;
				renderCom.material.color = c;
			}

			float sx = Mathf.Lerp(transform.localScale.x, scaleXTo, aPercent);
            float sy = Mathf.Lerp(transform.localScale.y, scaleYTo, aPercent);
            transform.localScale = new Vector3(sx, sy, 0);
		}

		void FitGround(Vector3 aNormal) {
			transform.rotation = Quaternion.FromToRotation(Vector3.right, aNormal);

			if (transform.eulerAngles.y != 0) {
				transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - 270);
			} else {
				transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - 90);
			}
		}
	}
}