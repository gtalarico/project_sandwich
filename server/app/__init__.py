'''import main Flask. This starts when run.py calls from app import app'''
import os
from flask import Flask
# from flask_assets import Bundle, Environment
# from flask_caching import Cache
# from flask_s3 import FlaskS3
# from flask_debugtoolbar import DebugToolbarExtension
#
# from app.assets import css_assets, js_assets
# from app.assets import css_chm, js_chm
# from app.logger import logger

# Config
app = Flask(__name__)
# compress = Compress()
# toolbar = DebugToolbarExtension()

app.config.update(dict(
    DEBUG = True,
    BASEDIR = os.path.abspath(os.path.dirname(__file__)),
    JSONIFY_PRETTYPRINT_REGULAR = False
    ))

# ASSETS
# assets = Environment(app)
# assets.debug = bool(int(os.getenv('ASSETS_DEBUG', False)))
# assets.register('css_assets', css_assets)
# assets.register('js_assets', js_assets)
# assets.register('css_chm', css_chm)
# assets.register('js_chm', js_chm)

# logger.debug('FLASK S3 ACTIVE: {}'.format(app.config['FLASKS3_ACTIVE']))
# logger.debug('FLASK S3 ASSETS ACTIVE: {}'.format(app.config['FLASK_ASSETS_USE_S3']))

from app import views
# from app import assets
