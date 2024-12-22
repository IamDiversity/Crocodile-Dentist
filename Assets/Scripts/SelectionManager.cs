using UnityEngine;
using UnityEngine.Serialization;

public class SelectionManager : MonoBehaviour
{
    [FormerlySerializedAs("toothLayer")]
    [SerializeField]
    private LayerMask _toothLayer;

    [FormerlySerializedAs("highlightMaterial")]
    [SerializeField]
    private Material _highlightMaterial;
    
    public Material _defaultMaterial;
    public Material _aiHighlightMaterial;

    private Ray _ray;
    private Camera _cam;
    private readonly float _maxDistanceClick = 100.0f;
    private Transform _selection;

    private GameObject hitObject;


    private void Awake()
    {
        // Get the main camera
        _cam = Camera.main;
    }

    void Update()
    {
        // IF : is round OR it's not player's turn OR player already clicked a tooth, return
        if (GameManager.instance?.GameState != EN_GameState.ROUND || 
            GameManager.instance?.PlayerTurn != EN_Players.PLAYER_01 || 
            GameManager.instance?.clicked == true) {
            return;
        }
        
        // Clear the previous selection and set the previous selected tooth to the default material 
        if (_selection) {
            MeshRenderer selectionRenderer = _selection.parent?.GetComponent<MeshRenderer>();
            selectionRenderer.material = _defaultMaterial;
            _selection = null;
        }

        // Create a ray from the camera to where the mouse position is
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

        // IF : we hit a tooth
        if (Physics.Raycast(ray, out RaycastHit hit, _maxDistanceClick, _toothLayer, QueryTriggerInteraction.Ignore)) {
            // Get the hit object
            hitObject = hit.collider.gameObject;
            // Check if it has the "Tooth" script
            if (hitObject.transform.parent.TryGetComponent<Tooth>(out Tooth tooth)) {
                // Check if that tooth is already "activated"
                if (!tooth.IsActivated()) {
                    // IF : not activated, set its material to the highlight one
                    SetMaterial(hit);
                }
            }
            // Handling the click on a tooth
            if (Input.GetMouseButtonDown(0)) {
                CheckForCollision();
            }
        }
    }

    // Set the material of the hit object (tooth) to the highlight one and
    // save the current selected tooth in "_selection"
    private void SetMaterial(RaycastHit hit)
    {
        Transform selection = hit.transform;
        MeshRenderer selectionRenderer = selection.parent?.GetComponent<MeshRenderer>();
                
        if (selectionRenderer) {
            selectionRenderer.material = _highlightMaterial;
        }
        _selection = selection;
    }

    
    private void CheckForCollision()
    {
        // Check if an object has been hit and if it has a parent
        GameObject parent = hitObject?.transform.parent?.gameObject;

        // IF : parent isn't null, try to get its "Tooth" script and call "OnPressed" on that tooth
        if (parent != null) {
            if (parent.TryGetComponent<Tooth>(out Tooth tooth)) {
                tooth.OnPressed();
            }
        }
    }

    // This "SetMaterial" is specific to AI when it's selecting a tooth as it plays a blink animation
    public void SetMaterial(Tooth tooth,Material material)
    {
        if (tooth.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer)) {
            meshRenderer.material = material;
        }
    }
}
