from django.shortcuts import render
from django.http import HttpResponse
from rest_framework.decorators import api_view,APIView
from rest_framework.response import Response
from django.contrib.auth.models import User
from .models import profile,Score
from django.db.models import F, ExpressionWrapper, FloatField
# Create your views here.
# @api_view(['GET'])
def home(request):
    return HttpResponse("hello World")

@api_view(['POST'])
def Profile(request):
    if request.method == 'POST':
        username = request.POST.get('name', '')
        phone_no = request.POST.get('number', '')

        # Check if the username already exists
        if User.objects.filter(username=username).exists():
            return Response({'error': 'Username already exists'}, status=400)

        # If the username is unique, proceed with creating the user and profile
        user = User.objects.create_user(username=username)
        user_model = User.objects.get(username=username)
        profile_obj, created = profile.objects.get_or_create(user=user_model, Phone_No=phone_no, name=username, wallet=0)

        if created:
            return Response(status=201)
        else:
            return Response({'error': 'Profile creation failed'}, status=500)
    
     
@api_view(['POST'])
def Wallet_Update(request):
    if request.method=='POST':
        ph_number = request.POST['number']
        wallet = request.POST['wallet']
        
        update_profile = profile.objects.filter(Phone_No=ph_number).first()
        
        if wallet:
            update_profile.wallet = wallet
        
        update_profile.save()
        
        data = {
            'name':update_profile.name,
            'ph_number':update_profile.Phone_No,
            'wallet':update_profile.wallet
        }
        
        return Response(data,status = 200)

@api_view(['POST'])
def UpdateScore(request):
    if request.method == 'POST':
        user_id = request.data.get('user_id')
        user_score = request.data.get('score')
        quiz_name = request.data.get('quiz_name')
        userName = request.data.get('name')

        if user_id is not None and user_score is not None and quiz_name is not None:
            # Check if the user with the specified quiz name exists
            user_exists = Score.objects.filter(user_id=user_id, quizName=quiz_name).exists()

            if not user_exists:
                # Create a new score entry for the user
                new_score = Score.objects.create(
                    user_id=user_id,
                    points=user_score,
                    quizName=quiz_name,
                    name = userName
                )

                return Response({'score_id': new_score.id}, status=201)
            else:
                return Response({'error': 'User already has a score entry for the specified quiz name'}, status=400)
        else:
            return Response({'error': 'Invalid input data'}, status=400)


@api_view(['POST'])
def GetRankedList(request):
    quiz_name = request.data.get('quizname')

    if not quiz_name:
        return Response({'error': 'Missing quizname in request body'}, status=400)

    # Assuming you have a Score model with fields 'points', 'user_id', 'id', and 'quizName'
    scores = Score.objects.filter(quizName=quiz_name).order_by('-points', 'id', 'user_id')

    # Create a list of dictionaries containing user information
    user_list = [
        {
            'user_id': score.user_id,
            'points': score.points,
            'name' : score.name
        }
        for score in scores
    ]

    return Response(user_list, status=200)



@api_view(['POST'])
def GetUserPosition(request):
    try:
        # Get the user_id and quiz_name from the POST data
        user_id = request.data.get('user_id')
        quiz_name = request.data.get('quiz_name')

        if not user_id or not quiz_name:
            return Response({'error': 'Missing user_id or quiz_name in request body'}, status=400)

        # Get the user score for the given user_id and quiz_name
        user_score = Score.objects.get(user_id=user_id, quizName=quiz_name)
        
        # Check if the user has made a claim
        if not user_score.claim:
            # Get the list of users for the quiz
            user_score.claim = True
            user_score.save()
            scores = Score.objects.filter(quizName=quiz_name).order_by('-points', 'id', 'user_id')
            
            # Find the position of the user in the list
            user_position = next((index + 1 for index, score in enumerate(scores) if score.user_id == user_id), None)
            
            # Return the total size of the list and the user's position
            return Response({
                'total_size': len(scores),
                'user_position': user_position,
                'claim' : user_score.claim
            }, status=200)
        else:
            # Return null if the claim is true
            return Response({'message': 'Claim is already made'}, status=200)
    except Score.DoesNotExist:
        return Response({'error': 'User not found'}, status=404)
