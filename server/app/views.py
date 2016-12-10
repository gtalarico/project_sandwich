from collections import OrderedDict
import uuid
import os
import json
import time

from flask import render_template, redirect, url_for, send_from_directory
from flask import request
from flask import abort, flash, jsonify

from app import app
# from app.logger import logger

PROJECT_FOLDER = os.path.dirname(os.path.dirname(app.config['BASEDIR']))
REVIT_FOLDER = os.path.join(PROJECT_FOLDER, 'revit')
JOB_FOLDER = os.path.join(PROJECT_FOLDER, 'jobs')
JOB_PENDING_FOLDER = os.path.join(JOB_FOLDER, 'pending')
JOB_DONE_FOLDER = os.path.join(JOB_FOLDER, 'done')

categories = [
    'OST_Floors',
    'OST_Furniture',
    'OST_FurnitureSystems',
    'OST_Levels',
    'OST_Rooms',
    'OST_RoomTags',
    'OST_Views',
    'OST_SpecialityEquipment',
    'OST_Walls',
]

@app.route('/')
@app.route('/index.html', methods=['GET'])
def index():

    return render_template('index.html',
                           categories=categories,
                           filepaths=get_demo_filepaths(),
                           done_job_ids=get_done_jobs())


@app.route('/makejob', methods=['GET'])
def create_job():
    category = request.args.get('category')
    filename = request.args.get('filename')
    istype = int(request.args.get('istype'))
    isnottype = int(request.args.get('isnottype'))
    job_id = str(time.time()).replace('.', '')
    # job_id = str(uuid.uuid4().int)[:10]
    filters = {'category': category}
    filters['istype'] = istype
    filters['isnottype'] = isnottype
    job_data = create_job(job_id, filename, filters)
    return jsonify(job_data)


@app.route('/getjob', methods=['GET'])
def check_job():
    job_id = request.args.get('job_id')
    job_data = get_job(job_id)
    return jsonify(job_data)


def get_job(job_id):
    job_path = os.path.join(JOB_DONE_FOLDER, job_id)
    if not os.path.exists(job_path):
        job_path = os.path.join(JOB_PENDING_FOLDER, job_id)
    with open(job_path) as fp:
        job_data = json.load(fp)
    return job_data


@app.route('/job/<string:job_id>', methods=['GET'])
def render_job(job_id):
    job_data = get_job(job_id)

    return render_template('viewer.html',
                           job=json.dumps(job_data),
                           job_id=job_id,
                           done_job_ids=get_done_jobs())


def get_done_jobs():
    job_path = os.path.join(JOB_DONE_FOLDER)
    jobs = []
    for job_id in reversed(os.listdir(job_path)):
        job_filepath = os.path.join(JOB_DONE_FOLDER, job_id)
        with open(job_filepath) as fp:
            job = json.load(fp)
            jobs.append(job)
    return jobs


def get_demo_filepaths():
    filepaths = []
    for root, subfolders, files in os.walk(PROJECT_FOLDER):
        for filename in files:
            if filename.lower().endswith('.rvt'):
                # filepaths.append(os.path.join(root, filename))
                filepaths.append(os.path.join(filename))
    return filepaths


def create_job(job_id, filename, filters):
    job_path = os.path.join(JOB_PENDING_FOLDER, job_id)
    revit_path = os.path.join(REVIT_FOLDER, filename)
    job_data = {'status': 'pending',
                'job_id': job_id,
                'action': 'get',
                'filepath': revit_path,
                'filters': filters}

    with open(job_path, 'w') as fp:
        json.dump(job_data, fp, indent=2)

    print('job_created: {}'.format(job_id))
    return job_data


# @app.route('/checkstatus/jobid', methods=['GET'])
# def search_api():
    # query = request.args.get('category')
#     if not query or query == '0':
#         return jsonify({'error': 'Invalid Query Param'})
    # return jsonify({'fake_data':query})


# This handles the static files form the .CHM content
# @app.route('/icons/<string:filename>', methods=["GET"])
# @app.route('/scripts/<string:filename>', methods=["GET"])
# @app.route('/styles/<string:filename>', methods=["GET"])
@app.route('/favicon.ico', methods=["GET"])
def chm_static_redirect(filename=None):
    path = '/static' + request.path
    return redirect(path, 301)
