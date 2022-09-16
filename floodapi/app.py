from flask import Flask, request
from flask_cors import CORS
import requests

app=Flask(__name__)
CORS(app)

@app.route("/")
def hello():
    return "Hello"

@app.route("/flood", methods=['POST'])
def flood():
    prompt_text="this room is fully flooded. many waves and water."
    strength=0.8
    file=None
    if 'file' in request.files:
        file=request.files['file']
    response=requests.post("https://sdb.pcuenca.net/i2i", files=dict(image=file), data={
        'prompt': prompt_text,
        'strength': strength
    })
    print(response.text)


if __name__=='__main__':
    app.run()