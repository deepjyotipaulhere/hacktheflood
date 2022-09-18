from flask import Flask, request, jsonify
from flask_cors import CORS
import requests

app=Flask(__name__)
CORS(app)

@app.route("/")
def hello():
    return "Hello"

@app.route("/flood", methods=['POST'])
def flood():
    prompt_text=None
    try:
        prompt_text=request.form["prompt"]
    except:
        prompt_text="this room is fully flooded. many waves and water."
    strength=0.8
    file=None
    if 'file' in request.files:
        file=request.files['file']
    response=requests.post("https://sdb.pcuenca.net/i2i", files=dict(image=file), data={
        'prompt': prompt_text,
        'strength': strength
    })
    return jsonify({"data":"data:image/png;base64,"+response.json()['images'][0]})


if __name__=='__main__':
    app.run()