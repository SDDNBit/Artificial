using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using Autohand;
using Constants = SoftBit.Utils.Constants;
using Autohand.Demo;

namespace SoftBit.Autohand.Custom
{
    [DefaultExecutionOrder(2)]
    public class HandDistanceGrabberCustom : MonoBehaviour
    {
        [Header("Hands")]
        [Tooltip("The primaryHand used to trigger pulling or flicking")]
        public Hand primaryHand;
        [Tooltip("This is important for catch assistance")]
        public Hand secondaryHand;

        [Header("Pointing Options")]
        public Transform forwardPointer;
        [Space]
        [Tooltip("Defaults to grabbable on start if none")]
        public LayerMask layers;

        [Header("Pull Options")]
        public bool useInstantPull = false;
        [Tooltip("If false will default to distance pull, set pullGrabDistance to 0 for instant pull on select")]
        public bool useFlickPull = false;


        [Tooltip("The magnitude of your hands angular velocity for \"flick\" to start")]
        [ShowIf("useFlickPull")]
        public float flickThreshold = 7f;


        [Tooltip("The amount you need to move your hand from the select position to trigger the grab")]
        [HideIf("useFlickPull")]
        public float pullGrabDistance = 0.1f;

        [Space]
        [Tooltip("If this is true the object will be grabbed when entering the radius")]
        public bool instantGrabAssist = true;
        [Tooltip("The radius around of thrown object")]
        public float catchAssistRadius = 0.1f;

        [AutoToggleHeader("Show Events")]
        public bool showEvents = true;

        [ShowIf("showEvents")]
        public UnityHandGrabEvent OnPull;
        [ShowIf("showEvents"), Tooltip("Targeting is started when object is highlighted")]
        public UnityHandGrabEvent StartTarget;
        [ShowIf("showEvents")]
        public UnityHandGrabEvent StopTarget;
        [Tooltip("Selecting is started when grab is selected on highlight object")]
        [ShowIf("showEvents")]
        public UnityHandGrabEvent StartSelect;
        [ShowIf("showEvents")]
        public UnityHandGrabEvent StopSelect;


        private List<CatchAssistData> catchAssisted;

        private DistanceGrabbable targetingDistanceGrabbable;
        private DistanceGrabbable selectingDistanceGrabbable;

        private float catchAssistSeconds = 3f;
        private bool grabbing;
        private Vector3 startPullPosition;
        private RaycastHit hit;
        private Quaternion lastRotation;
        private RaycastHit selectionHit;
        private float selectedEstimatedRadius;
        private bool lastInstantPull;

        private GameObject _hitPoint;
        private Coroutine catchAssistRoutine;
        private DistanceGrabbable catchAsistGrabbable;
        private CatchAssistData catchAssistData;

        private DistanceGrabbable hitGrabbable;
        private GrabbableChild hitGrabbableChild;

        [Tooltip("The primaryHand listeners to deactivate when this is grabbing")]
        private OpenXRHandControllerLink openXRHandControllerLink;

        GameObject hitPoint
        {
            get
            {
                if (!gameObject.activeInHierarchy)
                    return null;

                if (_hitPoint == null)
                {
                    _hitPoint = new GameObject();
                    _hitPoint.name = "Distance Hit Point";
                    return _hitPoint;
                }

                return _hitPoint;
            }
        }

        private void Awake()
        {
            openXRHandControllerLink = primaryHand.GetComponent<OpenXRHandControllerLink>();
        }

        private void Start()
        {
            catchAssisted = new List<CatchAssistData>();
            if (layers == 0)
                layers = LayerMask.GetMask(Hand.grabbableLayerNameDefault);

            if (useInstantPull)
                SetInstantPull();
        }

        private void OnEnable()
        {
            primaryHand.OnTriggerGrab += TryCatchAssist;
            if (secondaryHand != null)
                secondaryHand.OnTriggerGrab += TryCatchAssist;
            primaryHand.OnBeforeGrabbed += (hand, grabbable) => { StopTargeting(); CancelGrab(); };

        }

        private void OnDisable()
        {
            primaryHand.OnTriggerGrab -= TryCatchAssist;
            if (secondaryHand != null)
                secondaryHand.OnTriggerGrab -= TryCatchAssist;
            primaryHand.OnBeforeGrabbed -= (hand, grabbable) => { StopTargeting(); CancelGrab(); };

            if (catchAssistRoutine != null)
            {
                StopCoroutine(catchAssistRoutine);
                catchAssistRoutine = null;
                catchAsistGrabbable.grabbable.OnGrabEvent -= (hand, grabbable) => { if (catchAssisted.Contains(catchAssistData)) catchAssisted.Remove(catchAssistData); };
                catchAsistGrabbable.OnPullCanceled -= (hand, grabbable) => { if (catchAssisted.Contains(catchAssistData)) catchAssisted.Remove(catchAssistData); };
            }
        }

        private void Update()
        {
            CheckTargetAndHighlightIt();
            CheckDistanceGrabbable();
            if (lastInstantPull != useInstantPull)
            {
                if (useInstantPull)
                {
                    useFlickPull = false;
                    pullGrabDistance = 0;
                }
                lastInstantPull = useInstantPull;
            }
        }

        private void OnDestroy()
        {
            Destroy(hitPoint);
        }

        public virtual void GrabTarget()
        {
            if (targetingDistanceGrabbable != null)
            {
                grabbing = true;
                startPullPosition = primaryHand.transform.localPosition;
                lastRotation = transform.rotation;
                selectionHit = hit;
                if (catchAssistRoutine == null)
                {
                    hitPoint.transform.position = selectionHit.point;
                    hitPoint.transform.parent = selectionHit.transform;
                }
                selectingDistanceGrabbable = targetingDistanceGrabbable;
                selectedEstimatedRadius = Vector3.Distance(hitPoint.transform.position, selectingDistanceGrabbable.transform.position);
                selectingDistanceGrabbable?.StartSelecting?.Invoke(primaryHand, selectingDistanceGrabbable.grabbable);
                targetingDistanceGrabbable?.StopTargeting?.Invoke(primaryHand, selectingDistanceGrabbable.grabbable);
                targetingDistanceGrabbable = null;
                StartSelect?.Invoke(primaryHand, selectingDistanceGrabbable.grabbable);
                StopTargeting();
            }
        }

        public virtual void CancelGrab()
        {
            StopTargeting();
            grabbing = false;
            selectingDistanceGrabbable?.StopSelecting?.Invoke(primaryHand, selectingDistanceGrabbable.grabbable);
            if (selectingDistanceGrabbable != null)
            {
                StopSelect?.Invoke(primaryHand, selectingDistanceGrabbable.grabbable);
            }
            selectingDistanceGrabbable = null;
        }

        private void SetInstantPull()
        {
            useInstantPull = true;
        }

        private void CheckTargetAndHighlightIt()
        {
            if (primaryHand.holdingObj == null && Physics.SphereCast(forwardPointer.position, Constants.DistangeGrabRadius, forwardPointer.forward, out hit, Constants.HandDistangeGrabRange, layers))
            {
                if (!grabbing)
                {
                    if (hit.transform.CanGetComponent(out hitGrabbable))
                    {
                        if (hitGrabbable != targetingDistanceGrabbable)
                        {
                            StartTargeting(hitGrabbable);
                        }
                    }
                    else if (hit.transform.CanGetComponent(out hitGrabbableChild))
                    {
                        if (hitGrabbableChild.grabParent.transform.CanGetComponent(out hitGrabbable))
                        {
                            if (hitGrabbable != targetingDistanceGrabbable)
                            {
                                StartTargeting(hitGrabbable);
                            }
                        }
                    }
                }
            }
            else
            {
                StopTargeting();
            }
        }

        private void CheckDistanceGrabbable()
        {
            if (grabbing)
            {
                if (useFlickPull)
                {
                    TryFlickPull();
                }
                else
                {
                    TryDistancePull();
                }
            }
        }

        private void StartTargeting(DistanceGrabbable target)
        {
            if (target.enabled && primaryHand.CanGrab(target.grabbable))
            {
                if (targetingDistanceGrabbable != null)
                {
                    StopTargeting();
                }
                targetingDistanceGrabbable = target;
                targetingDistanceGrabbable?.StartTargeting?.Invoke(primaryHand, target.grabbable);
                StartTarget?.Invoke(primaryHand, target.grabbable);
            }
        }

        private void StopTargeting()
        {
            targetingDistanceGrabbable?.StopTargeting?.Invoke(primaryHand, targetingDistanceGrabbable.grabbable);
            if (targetingDistanceGrabbable != null)
            {
                StopTarget?.Invoke(primaryHand, targetingDistanceGrabbable.grabbable);
            }
            else if (selectingDistanceGrabbable != null)
            {
                StopTarget?.Invoke(primaryHand, selectingDistanceGrabbable.grabbable);
            }
            targetingDistanceGrabbable = null;
        }

        private void ActivatePull()
        {
            if (selectingDistanceGrabbable)
            {
                OnPull?.Invoke(primaryHand, selectingDistanceGrabbable.grabbable);
                selectingDistanceGrabbable.OnPull?.Invoke(primaryHand, selectingDistanceGrabbable.grabbable);
                if (selectingDistanceGrabbable.instantPull)
                {
                    selectingDistanceGrabbable.grabbable.body.velocity = Vector3.zero;
                    selectingDistanceGrabbable.grabbable.body.angularVelocity = Vector3.zero;
                    selectionHit.point = hitPoint.transform.position;
                    primaryHand.Grab(selectionHit, selectingDistanceGrabbable.grabbable);
                    CancelGrab();
                    selectingDistanceGrabbable?.CancelTarget();
                }
                else if (selectingDistanceGrabbable.grabType == DistanceGrabType.Velocity)
                {
                    catchAssistRoutine = StartCoroutine(StartCatchAssist(selectingDistanceGrabbable, selectedEstimatedRadius));
                    catchAsistGrabbable = selectingDistanceGrabbable;
                    selectingDistanceGrabbable.SetTarget(primaryHand.palmTransform);
                }
                else if (selectingDistanceGrabbable.grabType == DistanceGrabType.Linear)
                {
                    selectingDistanceGrabbable.grabbable.body.velocity = Vector3.zero;
                    selectingDistanceGrabbable.grabbable.body.angularVelocity = Vector3.zero;
                    selectionHit.point = hitPoint.transform.position;
                    primaryHand.Grab(selectionHit, selectingDistanceGrabbable.grabbable, GrabType.GrabbableToHand);
                    CancelGrab();
                    selectingDistanceGrabbable?.CancelTarget();

                }

                CancelGrab();
            }
        }

        private void TryDistancePull()
        {
            if (Vector3.Distance(startPullPosition, primaryHand.transform.localPosition) > pullGrabDistance)
            {
                ActivatePull();
            }
        }

        private void TryFlickPull()
        {
            Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(lastRotation);
            lastRotation = transform.rotation;
            var getAngle = 0f;
            Vector3 getAxis = Vector3.zero;
            deltaRotation.ToAngleAxis(out getAngle, out getAxis);
            getAngle *= Mathf.Deg2Rad;
            float speed = (getAxis * getAngle * (1f / Time.deltaTime)).magnitude;

            if (speed > flickThreshold || useInstantPull)
            {
                if (selectingDistanceGrabbable)
                {
                    ActivatePull();
                }
            }
        }

        private void TryCatchAssist(Hand hand, Grabbable grab)
        {
            for (int i = 0; i < catchAssisted.Count; i++)
            {
                var distance = Vector3.Distance(hand.palmTransform.position + hand.palmTransform.forward * catchAssistRadius, catchAssisted[i].grab.transform.position) - catchAssisted[i].estimatedRadius;
                if (distance < catchAssistRadius)
                {
                    Ray ray = new Ray(hand.palmTransform.position, hitPoint.transform.position - hand.palmTransform.position);
                    if (Physics.SphereCast(ray, 0.03f, out var catchHit, catchAssistRadius * 2, LayerMask.GetMask(Hand.grabbableLayerNameDefault, Hand.grabbingLayerName)))
                    {
                        if (catchHit.transform.gameObject == catchAssisted[i].grab.gameObject)
                        {
                            catchAssisted[i].grab.body.velocity = Vector3.zero;
                            catchAssisted[i].grab.body.angularVelocity = Vector3.zero;
                            hand.Grab(catchHit, catchAssisted[i].grab);
                            CancelGrab();
                        }
                    }
                }
            }
        }

        private IEnumerator StartCatchAssist(DistanceGrabbable grab, float estimatedRadius)
        {
            catchAssistData = new CatchAssistData(grab.grabbable, catchAssistRadius);
            catchAssisted.Add(catchAssistData);
            grab.grabbable.OnGrabEvent += (hand, grabbable) => { if (catchAssisted.Contains(catchAssistData)) catchAssisted.Remove(catchAssistData); };
            grab.OnPullCanceled += (hand, grabbable) => { if (catchAssisted.Contains(catchAssistData)) catchAssisted.Remove(catchAssistData); };

            if (instantGrabAssist)
            {
                bool cancelInstantGrab = false;
                var time = 0f;
                primaryHand.OnTriggerRelease += (hand, grabbable) => { cancelInstantGrab = true; };

                while (time < catchAssistSeconds && !cancelInstantGrab)
                {
                    time += Time.fixedDeltaTime;

                    if (TryCatch(primaryHand))
                        break;

                    bool TryCatch(Hand hand)
                    {
                        var distance = Vector3.Distance(hand.palmTransform.position + hand.palmTransform.forward * catchAssistRadius, grab.transform.position) - estimatedRadius;
                        if (distance < catchAssistRadius)
                        {
                            Ray ray = new Ray(hand.palmTransform.position, hitPoint.transform.position - hand.palmTransform.position);
                            var hits = Physics.SphereCastAll(ray, 0.03f, catchAssistRadius * 2, LayerMask.GetMask(Hand.grabbableLayerNameDefault, Hand.grabbingLayerName));
                            for (int i = 0; i < hits.Length; i++)
                            {
                                if (hits[i].transform.gameObject == grab.gameObject)
                                {
                                    grab.grabbable.body.velocity = Vector3.zero;
                                    grab.grabbable.body.angularVelocity = Vector3.zero;
                                    hand.Grab(hits[i], grab.grabbable);
                                    grab.CancelTarget();
                                    CancelGrab();
                                    return true;
                                }
                            }
                        }
                        return false;
                    }

                    yield return new WaitForEndOfFrame();
                }

                primaryHand.OnTriggerRelease -= (hand, grabbable) => { cancelInstantGrab = true; };

            }

            else
                yield return new WaitForSeconds(catchAssistSeconds);

            grab.grabbable.OnGrabEvent -= (hand, grabbable) => { if (catchAssisted.Contains(catchAssistData)) catchAssisted.Remove(catchAssistData); };
            grab.OnPullCanceled -= (hand, grabbable) => { if (catchAssisted.Contains(catchAssistData)) catchAssisted.Remove(catchAssistData); };
            if (catchAssisted.Contains(catchAssistData))
                catchAssisted.Remove(catchAssistData);

            catchAssistRoutine = null;
        }

        private void OnDrawGizmosSelected()
        {
            if (primaryHand)
                Gizmos.DrawWireSphere(primaryHand.palmTransform.position + primaryHand.palmTransform.forward * catchAssistRadius * 4 / 5f + primaryHand.palmTransform.up * catchAssistRadius * 1 / 4f, catchAssistRadius);
        }
    }

    struct CatchAssistData
    {
        public Grabbable grab;
        public float estimatedRadius;

        public CatchAssistData(Grabbable grab, float estimatedRadius)
        {
            this.grab = grab;
            this.estimatedRadius = estimatedRadius;
        }
    }
}
