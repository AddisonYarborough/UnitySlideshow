using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class SlideManager : MonoBehaviour
	{
	//All slides in the project, minus the fadeSlide
	public List<Transform> slides;
	//The panel that overlays all slides and changes from clear to black
	public Image fadeSlide;
	//Bools to control the fadePanel's color
	public bool fadeToBlack, fadeToClear;
	//The slide we're currently viewing
	public int currentSlide;

	void Start()
		{
		currentSlide = -1;//We set the currentSlide to -1 so that when it's incremented, it will be the first (0th) slide
		NextSlide(); //Proceed to the first slide
		}

	public void Update()
		{
		FadeSlides(); //Apply the color lerping to the fade panel, which overlays all slides
		ListenForInput(); //Check for arrow keys/space bar down so we can fade the fadePanel accordingly
		}

	void FadeSlides()
		{
		//Fade the fadePanel by lerping the color from it's current color to clear or black, depending on the bool
		if (fadeToBlack) fadeSlide.color += new Color(0, 0, 0, 1 * Time.deltaTime / 2);
		if (fadeToClear) fadeSlide.color -= new Color(0, 0, 0, 1 * Time.deltaTime / 2);
		if (fadeToClear && fadeSlide.color.a <= 0) fadeToClear = false; //Disable fadeToClear if we are already at minimum alpha (to avoid unnecessary updates)
		}

	void ListenForInput()
		{
		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow)) NextSlide(); //If we strike the spacebar or right arrow key, proceed to the next slide
		if (Input.GetKeyDown(KeyCode.LeftArrow)) PreviousSlide(); //If we strike the left arrow key, return to the previous slide
		if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit(); //If we strike the Esc key, quit the application
		}

	public void NextSlide()
		{
		if (currentSlide == slides.Count - 1) return; //If we're on the very last slide, ignore the rest of the method
		currentSlide++; //Increment the slide count
		StartCoroutine(SlideTransition(true)); //Start the slide transition (fade to black, then to clear)
		}


	IEnumerator SlideTransition(bool isNext) //If isNext is true, then we go to next slide, else we go to the previous slide
		{
		fadeToBlack = true; //Lerp the overlay panel to black
		yield return new WaitForSeconds(2); //Wait for lerping to finish
		//--Check if enabling previous or next slide..
		if (isNext) //If we're going to the next slide..
			{ //If we're not on the first slide, disable the previous slide
			if (currentSlide > 0) slides[currentSlide - 1].gameObject.SetActive(false);
			} else
			{ //If we're going to the previous slide.. and if the current slide is not the last slide - disable the following slide
			if (currentSlide < slides.Count - 1) slides[currentSlide + 1].gameObject.SetActive(false);
			}
		slides[currentSlide].gameObject.SetActive(true); //Enable the currentSlide
		fadeToBlack = false; //Disable the fadeToBlack bool since we're done fading to black
		fadeToClear = true; //Now that we've done our disabling/enabling, fade to clear
		yield return new WaitForSeconds(2); //Wait for the lerping to finish
		}
	
	void PreviousSlide()
		{
		if (currentSlide == 0) return; //If the current slide is the first slide, ignore the rest of this method
		currentSlide--; //Decrement the current slide
		StartCoroutine(SlideTransition(false)); //Start the slide transition (fade to black, then to clear)
		}
	}
