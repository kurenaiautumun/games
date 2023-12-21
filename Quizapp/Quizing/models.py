from django.db import models
from django.contrib.auth import get_user_model

User = get_user_model()
class profile(models.Model):
    user = models.ForeignKey(User, on_delete=models.CASCADE, null=True, blank=True)
    name = models.CharField(max_length=50)
    Phone_No = models.CharField(max_length = 15)
    wallet = models.BigIntegerField()

    def __str__(self):
        return self.user.username if self.user else 'No User'
    
