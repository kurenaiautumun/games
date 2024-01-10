from django.contrib import admin
from django.urls import path
from . import views
urlpatterns = [
    path('home/',views.home,name='home'),
    path('profile/',views.Profile,name='profile'),
    path('updates/',views.Wallet_Update,name='update'),
    path('updateScore/',views.UpdateScore,name='score'),
    path('getranklist/',views.GetRankedList,name='RankList')
]