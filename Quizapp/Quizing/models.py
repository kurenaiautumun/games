from django.db import models
from django.contrib.auth import get_user_model

User = get_user_model()
class profile(models.Model):
    user = models.ForeignKey(User, on_delete=models.CASCADE, null=True, blank=True)
    name = models.CharField(max_length=50)
    Phone_No = models.CharField(max_length = 15)
    wallet = models.CharField(max_length=50)

    def __str__(self):
        return self.user.username if self.user else 'No User'

class Score(models.Model):
    user_id = models.CharField(max_length=255, unique=True)
    points = models.IntegerField(default=0)
    time_between_points = models.FloatField(default=0)

    def __str__(self):
        return f"{self.user_id}'s Score"
    
