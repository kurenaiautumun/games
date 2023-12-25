from django.shortcuts import render
from django.http import HttpResponse
from rest_framework.decorators import api_view,APIView
from rest_framework.response import Response
from django.contrib.auth.models import User
from .models import profile
# Create your views here.
# @api_view(['GET'])
def home(request):
    return HttpResponse("hello World")

@api_view(['POST'])
def Profile(request):
    if request.method=='POST':
        username = request.POST['name']
        phone_no =request.POST['number']
        
        user = User.objects.create_user(username=username)
        user.save()
        
        user_model = User.objects.get(username=username)
        Profile = profile.objects.create(user = user_model,Phone_No = phone_no,name = username,wallet = 0)
        Profile.save()
        
        # userdata = {
        #     'username':user_model.username,
        #     'number': phone_no
        # }
        return Response(status = 201)
    
     
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