using UnityEngine;
using System.Collections.Generic;

public class Snake : MonoBehaviour
{
    // Direction of the snake's movement; defaults to moving right
    private Vector2 _direction = Vector2.right;

    // List to hold all segments of the snake, including the head
    private List<Transform> _segments = new List<Transform>();

    // Prefab for the snake's body segment
    public Transform _segmentPrefab;

    // Audio clips for snake actions
    public AudioSource HissSFX;    // Hiss sound effect
    public AudioSource GameOverSFX; // Game over sound effect
    public AudioSource EatSFX;    // Eating sound effect

    void Start()
    {
        // Add the snake's head (this object) as the first segment in the list
        _segments.Add(this.transform);
    }

    void Update()
    {
        // Prevent reversing direction:
        // Change direction only if the new direction isn't opposite to the current direction
        if (Input.GetKeyDown(KeyCode.W) && _direction != Vector2.down)
            _direction = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S) && _direction != Vector2.up)
            _direction = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A) && _direction != Vector2.right)
            _direction = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D) && _direction != Vector2.left)
            _direction = Vector2.right;

        // Randomly play the hiss sound effect based on a probability
        float x = 1;
        if (Random.Range(1.0f, 10.0f) == x)
        {
            HissSFX.Play();
        }
    }

    void FixedUpdate()
    {
        // Store the current position of the head before moving it
        Vector3 previousPosition = transform.position;

        // Move the head of the snake in the current direction
        transform.position = new Vector3(
            Mathf.Round(transform.position.x) + _direction.x, // Move in the x-direction
            Mathf.Round(transform.position.y) + _direction.y, // Move in the y-direction
            0.0f // Keep the z-position constant for a 2D game
        );

        // Update the position of each body segment
        for (int i = _segments.Count - 1; i > 0; i--)
        {
            // Each segment moves to the position of the segment ahead of it
            _segments[i].position = _segments[i - 1].position;
        }

        // Ensure the first body segment follows the head
        if (_segments.Count > 1)
        {
            _segments[1].position = previousPosition;
        }
    }

    // Function to grow the snake when it eats food
    private void Grow()
    {
        // Instantiate a new segment from the prefab
        Transform segment = Instantiate(_segmentPrefab);

        // Add the new segment to the list of segments
        _segments.Add(segment);

        // Place the new segment at the position of the last segment
        segment.position = _segments[_segments.Count - 1].position;

        // Play the eating sound effect
        EatSFX.Play();
    }

    // Function to reset the snake's state when the game is over
    private void ResetState()
    {
        // Destroy all body segments except the head
        for (int i = 1; i < _segments.Count; i++)
        {
            Destroy(_segments[i].gameObject);
        }

        // Clear the list and add only the head back
        _segments.Clear();
        _segments.Add(this.transform);

        // Reset the position of the head
        transform.position = Vector3.zero;

        // Reset the direction to the default (right)
        _direction = Vector2.right;

        // Play the game over sound effect
        GameOverSFX.Play();
    }

    // Detect collisions with other objects
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the snake collides with food, grow
        if (other.CompareTag("Food"))
        {
            Grow();
        }
        // If the snake collides with an obstacle, reset the game
        else if (other.CompareTag("Obstacle"))
        {
            ResetState();
        }
    }
}
