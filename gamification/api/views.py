from django.shortcuts import render
from rest_framework import viewsets
from .models import Objects,Points,User
from .serializers import ObjectsSerializers,PointsSerializers,UserSerializer
# Create your views here.

class UserView(viewsets.ModelViewSet):
    serializer_class=UserSerializer
    queryset=User.objects.all()

class ObjectsView(viewsets.ModelViewSet):
    serializer_class=ObjectsSerializers
    queryset=Objects.objects.all()

class PointsView(viewsets.ModelViewSet):
    serializer_class=PointsSerializers
    queryset=Points.objects.all()