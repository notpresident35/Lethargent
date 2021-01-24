
VAR has_cigar = false
VAR has_family_photo = false
VAR has_looped = false

-> makeItSnappyImOf

= makeItSnappyImOf
{!Make it snappy - I'm off the clock.|Hurry up! The cigar's still burnin'}
    * Why aren't you helping with the search?
        -> doesItPayInCigar 
    * Does the security system work?
        -> ofCourseIfItDidn 
    * { has_cigar } Here's your cigar.
        -> thereWeGoTheCode 

= doesItPayInCigar
Does it pay in cigars? No? Then get lost!
    + [Leave] Have it your way then...
        -> end 
    + {not has_looped} I'll leave in a moment; I just have one more question.
        ~ has_looped = true
        -> makeItSnappyImOf 
    + {has_family_photo} Your family depends on this investigation! You need to help!
        -> areYouTryingToSc 

= ofCourseIfItDidn
Of course. If it didn't, it wouldn't be here, now would it? [Context: The security system is, in fact, offline, and must be enabled by the player. This does not change this dialogue]
  + Would it protect me if I stayed here for the night? I'm worried about the gang...
        -> nobodyGetsInWith 
  + Is that why you live in here?
        -> thatAndThisChair 

= thereWeGoTheCode
Not bad. The code is <em>1234.</em> You can stay in the cafeteria. Note: This code is treated like a key item; the player does not have to memorize it, as the game will automatically use it.
    + [Leave] Thanks...
        -> end 
    + {not has_looped} Actually, I had one more question...
        ~ has_looped = true
        -> makeItSnappyImOf 

= end
End
    -> END

= nobodyGetsInWith
Nobody gets in without the code, not even the gang. Get me a quality cigar and you can sleep in the cafeteria if you're that paranoid. Context: a cigar is literally across the room from the Boss in plain sight
    + [Leave] I'm not paranoid...
        -> end 
    + { has_cigar } Funny; I already have one.
        -> thereWeGoTheCode 

= areYouTryingToSc
...Are you trying to scare me or something? 
Is that what this is? 
Threaten my family - is that how you're gonna get me to care about your precious expletive redacted] country? 
I got some news for ya pal! 
She could live on the [<em>expletive redacted] moon</em> for all I care! 
I don't give a [redacted] if she or anybody else in this [redacted] country dies, and you're out of your [redacted] mind if you think that a <em>picture</em> can buy me! 
<em>Get out of my office!</em>
    + [But what about saving yourself? You live in this compound!] But-
        -> nOW 
    + [Leave] Ok, ok, I'm going.
        -> end 

= thatAndThisChair
That and this chair is finest in class. Are we done here?
    + [Leave] Yes, that's all I needed.
        -> end 
    + Is there any way I could stay here for the night?
        -> youCanStay
    + {not has_looped} No, I had something else to ask...
        ~ has_looped = true
        -> makeItSnappyImOf 
        
= youCanStay
I always wondered if you were that paranoid. Well, now I know. Get me a quality cigar and you can sleep in the cafeteria. Context: a cigar is literally across the room from the Boss in plain sight
    + [Leave] I'm not paranoid...
        -> end 
    + { has_cigar } Funny; I already have one.
        -> thereWeGoTheCode 

= nOW
<strong><em>NOW!!</em></strong>
    -> end