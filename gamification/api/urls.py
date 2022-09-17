from .views import ObjectsView,PointsView,UserView
from rest_framework.routers import DefaultRouter
from rest_framework.urls import path
from django.urls import include

router=DefaultRouter()

router.register("users", UserView, basename='users')
router.register("objects", ObjectsView, basename='objects')
router.register("points", PointsView, basename='points')

urlpatterns=[
    path("", include(router.urls)),
    # path("leaderboard", leaderboard, name='leaderboard')
]