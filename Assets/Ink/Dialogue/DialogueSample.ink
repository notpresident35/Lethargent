// ---- Lethargent Dialogue ----
// Converted from original inklewriter URL:
// https://www.inklewriter.com/stories/42290
# title: Lethargent Dialogue
# author: John Kennedy (Dawdle)
// -----------------------------


-> chooseACharacter


==== chooseACharacter ====
Choose a character...
Legend:
[things in square brackets] are contextual writer's notes.
(things in parentheses) are statements that will show up when choosing dialogue but will be interrupted by the next character's statement
Things separated // by two slashes are randomly chosen (or context-sensitive) options that can be substituted for each other. This feature can be cut, but it makes dialogue more immersive, especially for looping choices.
  + Boss
        -> makeItSnappyImOf 

= makeItSnappyImOf
Make it snappy - I'm off the clock. // Hurry up! The cigar's still burnin'
  + Why aren't you helping with the search?
        -> doesItPayInCigar 
  + Does the security system work?
        -> ofCourseIfItDidn 
  + [If player has Cigar and has asked to stay] Here's your cigar.
        -> thereWeGoTheCode 

= doesItPayInCigar
Does it pay in cigars? No? Then get lost!
  + [Leave] Have it your way then...
        -> end 
  + [If player has photo of Boss' family] Your family depends on this investigation! You need to help!
        -> areYouTryingToSc 

= ofCourseIfItDidn
Of course. If it didn't, it wouldn't be here, now would it? [Context: The security system is, in fact, offline, and must be enabled by the player. This does not change this dialogue]
  + Would it protect me if I stayed here for the night? I'm worried about the gang...
        -> nobodyGetsInWith 
  + Is that why you live in here?
        -> thatAndThisChair 

= thereWeGoTheCode
There we go. The code is <em>1234.</em> You can stay in the cafeteria.
  + [Leave] Thanks...
        -> end 
  + Actually, I had one more question...
        -> makeItSnappyImOf 

= end
End
    -> END

= nobodyGetsInWith
Nobody gets in without the code, not even the gang. Get me a quality cigar and you can sleep in the cafeteria if you're that paranoid. [Context: a cigar is literally across the room from the Boss in plain sight]
  + [Leave] I'm not paranoid...
        -> end 
  + [If player has Cigar] Funny; I already have one.
        -> thereWeGoTheCode 

= areYouTryingToSc
...Are you trying to scare me or something? Is that what this is? Threaten my family - is that how you're gonna get me to care about your precious expletive redacted] country? I got some news for ya pal! She could live on the [<em>expletive redacted] moon</em> for all I care! I don't give a [redacted] if she or anybody else in this [redacted] country dies, and you're out of your [redacted] mind if you think that a <em>picture</em> can buy me! <em>Get out of my office!</em>
  + But- ("what about saving yourself? You live in this compound")
        -> nOW 
  + [Leave] Ok, ok, I'm going.
        -> end 

= thatAndThisChair
That and this chair is finest in class. Are we done here?
  + [Leave] Yes, that's all I needed.
        -> end 
  + No, I had something else to ask...
        -> makeItSnappyImOf 

= nOW
<strong><em>NOW!!</em></strong>
    -> end