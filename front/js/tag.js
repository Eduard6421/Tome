const tagRegex = /^[a-zA-Z]+/;
const maxTagLength = 20;
function checkTagWordRestraint(newTag)
{
	return tagRegex.test(newTag);
}

function checkTagLengthRestraint(newTag)
{
	return newTag.length <= maxTagLength;
}


$(document).ready(function(){

	$('.tag-input').keypress(function(event)
	{
		if(event.keyCode == 13)
		{
			let newTag = $('.tag-input').val();

			newTag = newTag.charAt(0).toUpperCase() + newTag.slice(1);

			if(!checkTagWordRestraint(newTag))
			{
				alert('Tag must contain only one word and only normal characters');
				return false;
			} 

			if(!checkTagLengthRestraint(newTag))
			{
				alert('Tag must containt at most 20 characters');
				return false;
			}	

			$(`<li class='tag'>${newTag}</li>`).insertBefore('.tag-input');	
		}
	});
});
