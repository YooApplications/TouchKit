using UnityEngine;
using System.Collections;


public class TKTouch
{
		public readonly int fingerId;
		public Vector2 position;
		public Vector2 deltaPosition;
		public float deltaTime;
		public int tapCount;
		public TouchPhase phase = TouchPhase.Ended;
		
    	public Vector2 previousPosition;
		public bool isImpossibleTouch;
	
		public Vector2 normalizedDeltaPosition
		{
				get { return position - previousPosition;}
		}
	
	#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
		// used to track mouse movement and fake touches
		private double _lastClickTime;
		private double _multipleClickInterval = 0.2;
	#endif
	
	
		public TKTouch( int fingerId )
		{
				// lock this TKTouch to the fingerId
				this.fingerId = fingerId;
		}
	
		public TKTouch populateWithTouch( Touch touch )
		{		
				isImpossibleTouch = (phase == TouchPhase.Moved && touch.phase == TouchPhase.Stationary && normalizedDeltaPosition.magnitude > 5f) || (isImpossibleTouch && touch.phase == TouchPhase.Stationary);
 		        
				
				if (touch.phase == TouchPhase.Began)
				{
						previousPosition = touch.position;
				} else
				{
						previousPosition = position;
				}
				
				position = touch.position;
				deltaPosition = touch.deltaPosition;
				deltaTime = touch.deltaTime;
				tapCount = touch.tapCount;
				phase = touch.phase;


				return this;
		}

	#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
		public TKTouch populateWithPosition( Vector3 currentPosition, TouchPhase touchPhase )
		{
				Vector2 currentPosition2d = new Vector2( currentPosition.x, currentPosition.y );
		
				deltaPosition = currentPosition2d - position;
				previousPosition = position;
        		switch( touchPhase )
				{
						case TouchPhase.Began:
								phase = TouchPhase.Began;
			
								// check for multiple clicks
								if( Time.time < _lastClickTime + _multipleClickInterval )
										tapCount++;
								else
										tapCount = 1;
                				deltaPosition = new Vector2( 0, 0 );
								_lastClickTime = Time.time;
								previousPosition = currentPosition2d;
								break;
						case TouchPhase.Stationary:
						case TouchPhase.Moved:
								if( deltaPosition.magnitude == 0 )
								{
										phase = TouchPhase.Stationary;   
								} else
								{
										phase = TouchPhase.Moved;  
								}
								break;
						case TouchPhase.Ended:
								phase = TouchPhase.Ended;
								break;
				}
				
            	position = currentPosition2d;
				return this;
		}
	
	
		public TKTouch populateFromMouse()
		{
				// do we have some input to work with?
				if( Input.GetMouseButtonUp( 0 ) || Input.GetMouseButton( 0 ) )
				{
						TouchPhase phase = TouchPhase.Moved;
						if( Input.GetMouseButtonUp( 0 ) )
								phase = TouchPhase.Ended;
						if( Input.GetMouseButtonDown( 0 ) )
								phase = TouchPhase.Began;
			
						var currentMousePosition = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
						this.populateWithPosition( currentMousePosition, phase );
				}
		
				return this;
		}
	#endif
	
		public override string ToString()
		{
				return string.Format( "[TKTouch] fingerId: {0}, phase: {1}, position: {2}", fingerId, phase, position );
		}
    
}