using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SlideManager : MonoBehaviour {
    //All _slides in the project, minus the _fadeSlide
    [SerializeField]
    private List<Transform> _slides = default;

    //The panel that overlays all _slides and changes from clear to black
    [SerializeField]
    private Image _fadeSlide = default;

    [Header("Config Values")]
    [SerializeField, Tooltip("The duration (in seconds) over which the fade slide will fade in / out")]
    private float _fadeDuration = 0.75f;

    [SerializeField, Tooltip("All key codes that will move to the next slide if pressed")]
    private KeyCode[] _nextSlideKeyCodes = {
        KeyCode.D,
        KeyCode.RightArrow,
        KeyCode.Space
    };

    [SerializeField, Tooltip("All key codes that will move to the previous slide if pressed")]
    private KeyCode[] _previousSlideKeyCodes = {
        KeyCode.A,
        KeyCode.LeftArrow,
    };

    //The slide we're currently viewing
    private int _currentSlide = -1;

    // Whether the fade slide is currently fading
    private bool _isTransitioning = false;

    private void Start() {
        // Set our fade to black slide to black so that the audience can not see the first slide
        _fadeSlide.color = Color.black;
    }

    public void Update() {
        //Check for arrow keys/space bar down so we can fade the fadePanel accordingly
        ListenForInput();
    }

    private void ListenForInput() {
        // Ignore input if we're in the middle of a transition
        if (_isTransitioning) {
            return;
        }

        //If we strike the space bar or right arrow key, proceed to the next slide
        if (_nextSlideKeyCodes.Any(Input.GetKeyDown)) {
            NextSlide();
        }

        //If we strike the left arrow key, return to the previous slide
        if (_previousSlideKeyCodes.Any(Input.GetKeyDown)) {
            PreviousSlide();
        }

        //If we strike the Esc key, quit the application
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    private void NextSlide() {
        //If we're on the very last slide already..
        if (_currentSlide == _slides.Count - 1) {
            // Exit early
            return;
        }

        //Increment the slide count
        _currentSlide++;

        // Transition to the next slide
        StartCoroutine(SlideTransition());
    }


    private IEnumerator SlideTransition() {
        // Mark our fading slide as currently fading
        _isTransitioning = true;

        // Fade to black
        yield return StartCoroutine(FadeToTargetColor(targetColor: Color.black));

        // Set only our current slide active - and all others inactive
        _slides.ForEach(slide => slide.gameObject.SetActive(_slides.IndexOf(slide) == _currentSlide));

        // Fade to clear
        yield return StartCoroutine(FadeToTargetColor(targetColor: Color.clear));

        // Mark our fading slide as no longer fading
        _isTransitioning = false;
    }

    private void PreviousSlide() {
        //If the current slide is the very first slide, ignore the rest of this method
        if (_currentSlide == 0) {
            return;
        }

        //Decrement the current slide
        _currentSlide--;

        // Transition to the previous slide
        StartCoroutine(SlideTransition());
    }

    private IEnumerator FadeToTargetColor(Color targetColor) {
        // The total amount of seconds that has elapsed since the start of our lerp sequence
        float elapsedTime = 0.0f;

        // The color of our fade panel at the start of the lerp sequence
        Color startColor = _fadeSlide.color;

        // While we haven't reached the end of the lerp sequence..
        while (elapsedTime < _fadeDuration) {
            // Increase our elapsed time
            elapsedTime += Time.deltaTime;

            // Perform a lerp to our target color
            _fadeSlide.color = Color.Lerp(startColor, targetColor, elapsedTime / _fadeDuration);

            // Wait for the next frame
            yield return null;
        }
    }
}
